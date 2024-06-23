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
    public class AgendaController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public AgendaController(AppDbContext appDbContext)
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


        [HttpGet("agendaEvents")]
        [Authorize]
        public async Task<IActionResult> GetAgendaEvents()
        {
            try
            {
                var userId = GetUserIdFromToken();

                var eventsWithoutAgenda = await _appDbContext.Events
                    .Where(e => e.ClientId == userId && e.Agenda == null && e.EndDateTime > DateTime.Now)
                    .Select(e => new { e.EventId, e.Name })
                    .ToListAsync();

                return Ok(eventsWithoutAgenda);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpPost("SaveAgenda/{eventId}")]
        [Authorize]
        public async Task<IActionResult> SaveAgenda([FromBody] AgendaDTO agendaObj, [FromRoute] int eventId)
        {
            try
            {
                var userId = GetUserIdFromToken();

                if (agendaObj == null)
                {
                    throw new Exception("Agenda cannot be null!");
                }

                var newAgenda = new Agenda
                {
                    Date = agendaObj.Date,
                    Title = agendaObj.Title,
                    Description = agendaObj.Description,
                    EventId = eventId,
                    AgendaTasks = agendaObj.Tasks?.Select(item =>
                    {
                        if (!TimeOnly.TryParse(item.Time, out var time))
                        {
                            throw new Exception($"Invalid time format for task: {item.TaskName}");
                        }

                        return new AgendaTask
                        {
                            Time = time,
                            TaskName = item.TaskName,
                            TaskDescription = item.TaskDescription
                        };
                    }).ToList()
                };

                _appDbContext.Agenda.Add(newAgenda);
                await _appDbContext.SaveChangesAsync();

                return Ok(new { Message = "Agenda saved successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("{eventId}")]
        [Authorize]
        public async Task<IActionResult> GetAgendaByEventId([FromRoute] int eventId)
        {
            try
            {
                var userId = GetUserIdFromToken();

                var agenda = await _appDbContext.Agenda
                    .Include(c => c.AgendaTasks)
                    .Include(c => c.Event)
                    .FirstOrDefaultAsync(c => c.EventId == eventId && c.Event!.ClientId == userId);


                if (agenda == null)
                {
                    var event1 = await _appDbContext.Events.FirstOrDefaultAsync(e => e.ClientId == userId && e.EventId == eventId);

                    return NotFound(new { Message = "Agenda not found!.", EventName = event1!.Name! });
                }

                var agendaDTO = new AgendaDTO
                {
                    Date = agenda.Date,
                    Title = agenda.Title,
                    Description = agenda.Description,
                    Tasks = agenda.AgendaTasks?.Select(task => new TaskDTO
                    {
                        Time = task.Time.ToString(),
                        TaskName = task.TaskName,
                        TaskDescription = task.TaskDescription
                    }).ToList()
                };

                return Ok(new { agenda = agendaDTO, agendaId = agenda.AgendaId, EventName = agenda!.Event!.Name });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPut("updateAgenda/{agendaId}")]
        [Authorize]
        public async Task<IActionResult> UpdateAgenda([FromBody] AgendaDTO agendaObj, [FromRoute] int agendaId)
        {
            try
            {
                var userId = GetUserIdFromToken();

                if (agendaObj == null)
                {
                    return BadRequest(new { Message = "Agenda cannot be null!" });
                }

                var existingAgenda = await _appDbContext.Agenda
                    .Include(c => c.AgendaTasks)
                    .Include(c => c.Event)
                    .FirstOrDefaultAsync(c => c.AgendaId == agendaId && c.Event!.ClientId == userId);

                if (existingAgenda == null)
                {
                    return NotFound(new { Message = "Agenda not found for the given ID." });
                }

                // Update agenda fields
                existingAgenda.Date = agendaObj.Date;
                existingAgenda.Title = agendaObj.Title;
                existingAgenda.Description = agendaObj.Description;

                // Remove existing agenda tasks
                _appDbContext.AgendaTask.RemoveRange(existingAgenda!.AgendaTasks!);

                // Add updated tasks
                existingAgenda.AgendaTasks = agendaObj.Tasks?.Select(item =>
                {
                    if (!TimeOnly.TryParse(item.Time, out var time))
                    {
                        throw new Exception($"Invalid time format for task: {item.TaskName}");
                    }

                    return new AgendaTask
                    {
                        Time = time,
                        TaskName = item.TaskName,
                        TaskDescription = item.TaskDescription,
                        AgendaId = agendaId
                    };
                }).ToList() ?? new List<AgendaTask>();

                // Save changes to the database
                await _appDbContext.SaveChangesAsync();

                return Ok(new { Message = "Agenda updated successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

    }
}
