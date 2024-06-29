using eventify_backend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eventify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public ChatController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet("{soRId}")]
        public async Task<IActionResult> GetPhoneNumber([FromRoute] int soRId)
        {
            try
            {
                var item = await _appDbContext.ServiceAndResources.Include(s=> s.Vendor).FirstOrDefaultAsync(s=> s.SoRId == soRId);

                var Phone = item!.Vendor?.Phone;

                if(Phone == null)
                {
                    return NotFound();
                }

                return Ok(new { Phone = Phone});
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
