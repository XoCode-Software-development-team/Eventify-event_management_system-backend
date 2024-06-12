using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using xocode_backend.Models;
using xocode_backend.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace xocode_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventDbContext _eventDbContext;

        public EventController(EventDbContext eventDbContext)
        {
            _eventDbContext = eventDbContext;
        }

        [HttpGet]
        [Route("GetEvent")]
        public async Task<IActionResult> GetEvents()
        {
            try
            {
                var events = await _eventDbContext.Event.ToListAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database.");
            }
        }

        [HttpPost]
        [Route("AddEvent")]
        public async Task<IActionResult> AddEvent(Events objEvent)
        {
            try
            {
                _eventDbContext.Event.Add(objEvent);
                await _eventDbContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetEvents), new { id = objEvent.Id }, objEvent);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new event record.");
            }
        }

        [HttpPatch]
        [Route("UpdateEvent/{Id}")]
        public async Task<IActionResult> UpdateEvent(int Id, [FromBody] Events objEvent)
        {
            try
            {
                if (objEvent == null)
                {
                    return BadRequest("Invalid event data");
                }

                if (Id != objEvent.Id)
                {
                    return BadRequest("Event ID mismatch");
                }

                _eventDbContext.Entry(objEvent).State = EntityState.Modified;
                await _eventDbContext.SaveChangesAsync();

                return Ok(new { message = "Event updated successfully" });
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating event record.");
            }
        }

        [HttpDelete]
        [Route("DeleteEvent/{Id}")]
        public async Task<IActionResult> DeleteEvent(int Id)
        {
            try
            {
                var event1 = await _eventDbContext.Event.FindAsync(Id);
                if (event1 == null)
                {
                    return NotFound("Event not found");
                }

                _eventDbContext.Entry(event1).State = EntityState.Deleted;
                await _eventDbContext.SaveChangesAsync();

                return Ok(new { message = "Event deleted successfully" });
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting event record.");
            }
        }


    }
}
