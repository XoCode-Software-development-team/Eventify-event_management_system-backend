using eventify_backend.Data;
using eventify_backend.DTOs;
using eventify_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eventify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChecklistController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public ChecklistController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null)
            {
                throw new Exception("User ID is missing in the token.");
            }

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                throw new Exception("Invalid User ID in token.");
            }

            return userId;
        }

        [HttpGet("checklistEvents")]
        [Authorize]
        public async Task<IActionResult> GetChecklistEvents()
        {
            try
            {
                var userId = GetUserIdFromToken();

                var eventsWithoutChecklist = await _appDbContext.Events
                    .Where(e => e.ClientId == userId && e.Checklist == null && e.EndDateTime > DateTime.Now)
                    .Select(e => new { e.EventId, e.Name })
                    .ToListAsync();

                return Ok(eventsWithoutChecklist);
            }
            catch (Exception ex)
            {
                // Log the exception message here if needed
                return StatusCode(500, ex.ToString());
            }
        }



        [HttpPost("saveChecklist/{eventId}")]
        [Authorize]
        public async Task<IActionResult> SaveChecklist([FromBody] ChecklistDTO checklistObj, [FromRoute] int eventId)
        {
            try
            {
                var userId = GetUserIdFromToken();

                if (checklistObj == null)
                {
                    throw new Exception("Checklist cannot be null!");
                }

                var newChecklist = new Checklist
                {
                    Date = checklistObj.Date,
                    Title = checklistObj.Title,
                    Description = checklistObj.Description,
                    EventId = eventId,
                    ChecklistTasks = checklistObj.Tasks?.Select(item => new ChecklistTask
                    {
                        Checked = item.Checked,
                        TaskName = item.TaskName,
                        TaskDescription = item.TaskDescription
                    }).ToList()
                };

                _appDbContext.Checklist.Add(newChecklist);
                await _appDbContext.SaveChangesAsync();

                return Ok(new { Message = "Checklist saved successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("{eventId}")]
        [Authorize]
        public async Task<IActionResult> GetChecklistByEventId([FromRoute] int eventId)
        {
            try
            {
                var userId = GetUserIdFromToken();

                var checklist = await _appDbContext.Checklist
                    .Include(c => c.ChecklistTasks)
                    .Include(c => c.Event)
                    .FirstOrDefaultAsync(c => c.EventId == eventId && c.Event!.ClientId == userId);


                if (checklist == null)
                {
                    var event1 = await _appDbContext.Events.FirstOrDefaultAsync(e => e.ClientId == userId && e.EventId == eventId);

                    return NotFound(new { Message = "Checklist not found!.", EventName = event1!.Name! });
                }

                var checklistDTO = new ChecklistDTO
                {
                    Date = checklist.Date,
                    Title = checklist.Title,
                    Description = checklist.Description,
                    Tasks = checklist.ChecklistTasks?.Select(task => new TaskDTO
                    {
                        Checked = task.Checked,
                        TaskName = task.TaskName,
                        TaskDescription = task.TaskDescription
                    }).ToList()
                };

                return Ok(new { checklist = checklistDTO, checklistId = checklist.ChecklistId, EventName = checklist!.Event!.Name });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPut("updateChecklist/{checklistId}")]
        [Authorize]
        public async Task<IActionResult> UpdateChecklist([FromBody] ChecklistDTO checklistObj, [FromRoute] int checklistId)
        {
            try
            {
                var userId = GetUserIdFromToken();

                if (checklistObj == null)
                {
                    return BadRequest(new { Message = "Checklist cannot be null!" });
                }

                var existingChecklist = await _appDbContext.Checklist
                    .Include(c => c.ChecklistTasks)
                    .Include(c => c.Event)
                    .FirstOrDefaultAsync(c => c.ChecklistId == checklistId && c!.Event!.ClientId == userId);

                if (existingChecklist == null)
                {
                    return NotFound(new { Message = "Checklist not found for the given ID." });
                }

                // Update the checklist properties
                existingChecklist.Date = checklistObj.Date;
                existingChecklist.Title = checklistObj.Title;
                existingChecklist.Description = checklistObj.Description;

                // Clear the existing tasks and add the new ones
                _appDbContext.ChecklistTask.RemoveRange(existingChecklist!.ChecklistTasks!);

                existingChecklist.ChecklistTasks = checklistObj.Tasks?.Select(item => new ChecklistTask
                {
                    Checked = item.Checked,
                    TaskName = item.TaskName,
                    TaskDescription = item.TaskDescription,
                    ChecklistId = checklistId
                }).ToList();

                await _appDbContext.SaveChangesAsync();

                return Ok(new { Message = "Checklist updated successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

    }
}
