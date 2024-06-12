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
    }
}
