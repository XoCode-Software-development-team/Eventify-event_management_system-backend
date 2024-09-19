using eventify_backend.Data;
using eventify_backend.DTOs;
using eventify_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;

namespace xocode_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly AppDbContext _eventDbContext;
        private readonly NotificationService _notificationService;

        public EventController(AppDbContext eventDbContext, NotificationService notificationService)
        {
            _eventDbContext = eventDbContext;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Route("GetEvent")]
        [Authorize]

        public async Task<IActionResult> GetAllEvent()
        {
            try
            {
                // Extract userId from the JWT token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Message = "User ID is missing in the token." });
                }

                var userId = Guid.Parse(userIdClaim.Value);

                var events = await _eventDbContext.Events
                    .Where(e => e.ClientId == userId && e.EndDateTime >= DateTime.Now)
                    .Select(e => new
                    {
                        EventId = e.EventId,
                        Name = e.Name,
                    }).ToListAsync();

                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPastEvent")]
        [Authorize]

        public async Task<IActionResult> GetPastEvent()
        {
            try
            {
                // Extract userId from the JWT token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Message = "User ID is missing in the token." });
                }

                var userId = Guid.Parse(userIdClaim.Value);

                var events = await _eventDbContext.Events
                    .Where(e => e.ClientId == userId && e.EndDateTime < DateTime.Now)
                    .Select(e => new
                    {
                        EventId = e.EventId,
                        Name = e.Name,
                    }).ToListAsync();

                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetEvent/{eventId}")]
        [Authorize]
        public async Task<IActionResult> GetEventById(int eventId)
        {
            try
            {
                // Extract userId from the JWT token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Message = "User ID is missing in the token." });
                }

                var userId = Guid.Parse(userIdClaim.Value);

                var events = await _eventDbContext.Events
                    .Where(e => e.ClientId == userId && e.EventId == eventId)
                    .Select(e => new
                    {
                        EventId = e.EventId,
                        Name = e.Name,
                        StartDateTime = e.StartDateTime,
                        EndDateTime = e.EndDateTime,
                        Description = e.Description,
                        Location = e.Location,
                        GuestCount = e.GuestCount,
                        Thumbnail = e.Thumbnail,
                        Services = e.EventSoRApproves!
                            .Where(s => s.ServiceAndResource is Service)
                            .Select(s => new
                            {
                                ServiceId = s.ServiceAndResource!.SoRId,
                                Name = s.ServiceAndResource.Name,
                                CompanyName = s.ServiceAndResource.Vendor!.CompanyName,
                                OverallRate = s.ServiceAndResource.OverallRate,
                                Image = s.ServiceAndResource.VendorRSPhotos!.FirstOrDefault()!.Image, // Select the first image
                                isApprove = s.IsApprove
                            }).ToList(),
                        Resources = e.EventSoRApproves!
                            .Where(r => r.ServiceAndResource is Resource)
                            .Select(r => new
                            {
                                ResourceId = r.ServiceAndResource!.SoRId,
                                Name = r.ServiceAndResource.Name,
                                CompanyName = r.ServiceAndResource.Vendor!.CompanyName,
                                OverallRate = r.ServiceAndResource.OverallRate,
                                Image = r.ServiceAndResource.VendorRSPhotos!.FirstOrDefault()!.Image, // Select the first image
                                isApprove = r.IsApprove
                            }).ToList()
                    })
                    .ToListAsync();

                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving data from the database: {ex.Message}");
            }
        }




        [HttpPost]
        [Route("AddEvent")]
        [Authorize]
        public async Task<IActionResult> AddEvent(Event objEvent)
        {
            try
            {
                // Extract userId from the JWT token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Message = "User ID is missing in the token." });
                }

                var userId = Guid.Parse(userIdClaim.Value);

                // Set the ClientId for the event
                objEvent.ClientId = userId;

                _eventDbContext.Events.Add(objEvent);
                await _eventDbContext.SaveChangesAsync();

                // Return the newly created event ID
                return Ok(new { EventId = objEvent.EventId });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new event record.");
            }
        }


        [HttpPut]
        [Route("UpdateEvent/{eventId}")]
        [Authorize]
        public async Task<IActionResult> UpdateEvent(int eventId, [FromBody] Event objEvent)
        {
            try
            {
                // Extract userId from the JWT token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Message = "User ID is missing in the token." });
                }

                var userId = Guid.Parse(userIdClaim.Value);

                if (objEvent == null)
                {
                    return BadRequest(new { Message = "Invalid event data." });
                }

                // Check if the event exists in the database
                var existingEvent = await _eventDbContext.Events.FindAsync(eventId);
                if (existingEvent == null)
                {
                    return NotFound(new { Message = "Event not found." });
                }

                // Optionally, check if the user is authorized to update this event
                // Assuming there's a UserId property in the Event entity
                if (existingEvent.ClientId != userId)
                {
                    return Forbid();
                }

                // Update the event properties
                existingEvent.Name = objEvent.Name;
                existingEvent.StartDateTime = objEvent.StartDateTime;
                existingEvent.EndDateTime = objEvent.EndDateTime;
                existingEvent.Description = objEvent.Description;
                existingEvent.Location = objEvent.Location;
                existingEvent.GuestCount = objEvent.GuestCount;

                if (objEvent.Thumbnail != null)
                {
                    existingEvent.Thumbnail = objEvent.Thumbnail;
                }

                _eventDbContext.Entry(existingEvent).State = EntityState.Modified;
                await _eventDbContext.SaveChangesAsync();

                return Ok(new { Message = "Event updated successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception (ex) as needed
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error updating event record.", Detail = ex.Message });
            }
        }


        [HttpDelete]
        [Route("DeleteEvent/{Id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEvent(int Id)
        {
            try
            {
                // Extract userId from the JWT token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Message = "User ID is missing in the token." });
                }

                var userId = Guid.Parse(userIdClaim.Value);

                // Find the event to be deleted
                var event1 = await _eventDbContext.Events.FirstOrDefaultAsync(e => e.EventId == Id && e.ClientId == userId);
                if (event1 == null)
                {
                    return NotFound(new { Message = "Event not found." });
                }

                // Delete the event
                _eventDbContext.Events.Remove(event1);
                await _eventDbContext.SaveChangesAsync();

                // Find the next event for the user
                var nextEvent = await _eventDbContext.Events
                    .Where(e => e.ClientId == userId)
                    .OrderBy(e => e.EventId)
                    .FirstOrDefaultAsync();

                return Ok(new
                {
                    Message = "Event deleted successfully.",
                    NextEventId = nextEvent?.EventId ?? 0,
                });
            }
            catch (Exception ex)
            {
                // Log the exception (ex) as needed
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "Error deleting event record.",
                    Detail = ex.Message
                });
            }
        }

        [HttpGet("getEvents/{soRId}")]
        [Authorize]
        public async Task<IActionResult> GetAllEventBySoRId(int soRId)
        {
            try
            {
                // Extract userId from the JWT token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Message = "User ID is missing in the token." });
                }

                var userId = Guid.Parse(userIdClaim.Value);

                var events = await _eventDbContext.Events
                            .Where(e => e.ClientId == userId && e.EndDateTime >= DateTime.Now)
                            .Select(e => new
                            {
                                EventId = e.EventId,
                                Name = e.Name,
                                IsInVendorSr = _eventDbContext.EventSr.Any(v => v.SORId == soRId && v.Id == e.EventId),
                                IsPending = _eventDbContext.EventSoRApproves.Any(es => es.SoRId == soRId && es.EventId == e.EventId && es.IsApprove == false)
                            }).ToListAsync();

                return Ok(events);
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"{ex.Message}" });
            }
        }

        [HttpPost("AddServiceResource/{soRId}")]
        [Authorize]
        public async Task<IActionResult> AddServiceResourceToEvent([FromRoute] int soRId, [FromBody] EventSoRDTO eventSoR)
        {
            try
            {
                // Extract userId from the JWT token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Message = "User ID is missing in the token." });
                }

                var userId = Guid.Parse(userIdClaim.Value);

                var service = await _eventDbContext.ServiceAndResources.FirstOrDefaultAsync(s => s.SoRId == soRId);

                var user = await _eventDbContext.Clients.FirstOrDefaultAsync(u => u.UserId == userId);

                if (service == null || user == null)
                {
                    return BadRequest(new { Message = "Service or User not found" });
                }

                if (eventSoR == null || (eventSoR.Added?.Length == 0 && eventSoR.Removed?.Length == 0))
                {
                    return BadRequest(new { Message = "Invalid request data." });
                }

                if (eventSoR.Added?.Length > 0)
                {
                    foreach (var itemId in eventSoR.Added)
                    {
                        var eventSoRApprove = new EventSoRApprove
                        {
                            EventId = itemId,
                            IsApprove = false,
                            SoRId = soRId,
                            TimeStamp = DateTime.Now
                        };

                        _eventDbContext.EventSoRApproves.Add(eventSoRApprove);


                        // Retrieve event details
                        var eventDetails = await _eventDbContext.Events
                            .FirstOrDefaultAsync(e => e.EventId == itemId);

                        if (eventDetails != null)
                        {

                            var message = $"{service.Name} {(service is Service ? "service" : "resource")} is added to {eventDetails.Name} event by {user.FirstName}.";
                            await _notificationService.CreateIndividualNotification(message, service.VendorId);

                        }
                    }

                    await _eventDbContext.SaveChangesAsync();

                }

                if (eventSoR.Removed?.Length > 0)
                {
                    foreach (var itemId in eventSoR.Removed)
                    {
                        var eventSrApp = await _eventDbContext.EventSoRApproves.FirstOrDefaultAsync(e => e.EventId == itemId);

                        if (eventSrApp != null)
                        {
                            eventSrApp.IsApprove = false;
                            _eventDbContext.EventSoRApproves.Update(eventSrApp);
                        }

                        // Retrieve event details
                        var eventDetails = await _eventDbContext.Events
                            .FirstOrDefaultAsync(e => e.EventId == itemId);

                        if (eventDetails != null)
                        {

                            var message = $"{service.Name} {(service is Service ? "service" : "resource")} is removed from {eventDetails.Name} event by {user.FirstName}.";
                            await _notificationService.CreateIndividualNotification(message, service.VendorId);

                        }
                    }

                    await _eventDbContext.SaveChangesAsync();
                }

                return Ok(new { Message = "Operation completed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"{ex.Message}" });
            }
        }


        [HttpGet("checkEvent/{soRId}/{eventId}")]
        [Authorize]
        public async Task<IActionResult> CheckForEvents([FromRoute] int soRId, [FromRoute] int eventId)
        {
            try
            {
                // Extract userId from the JWT token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Message = "User ID is missing in the token." });
                }

                var userId = Guid.Parse(userIdClaim.Value);

                // Fetch the event with the given eventId
                var currentEvent = await _eventDbContext.Events
                    .Include(e => e.EventSoRApproves)
                    .Include(e => e.EventSRs)
                    .FirstOrDefaultAsync(e => e.EventId == eventId);

                if (currentEvent == null)
                {
                    return Ok(new { Message = "No event found for the given eventId" });
                }

                // Fetch other events associated with the given SoRId
                var otherEvents = await _eventDbContext.Events
                    .Include(e => e.EventSoRApproves)
                    .Include(e => e.EventSRs)
                    .Where(e => (e.EventSoRApproves!.Any(esr => esr.SoRId == soRId) || e.EventSRs!.Any(e => e.SORId == soRId)) && e.EventId != eventId)
                    .ToListAsync();

                var overlappingEventNames = new HashSet<string>();

                // Check for overlapping events or events on the same date
                foreach (var otherEvent in otherEvents)
                {
                    if (currentEvent.StartDateTime <= otherEvent.EndDateTime && currentEvent.EndDateTime >= otherEvent.StartDateTime)
                    {
                        overlappingEventNames.Add(otherEvent.Name!);
                    }
                }

                // Send notification if overlaps or same date events are found
                if (overlappingEventNames.Any())
                {
                    var message = $"{currentEvent.Name} overlaps with {string.Join(", ", overlappingEventNames)} on the same date.";
                    await _notificationService.CreateIndividualNotification(message, userId);
                    return Ok(new { Message = "Overlapping or same date events found", Details = message });
                }

                return Ok(new { Message = "No overlapping or same date events found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }










    }
}
