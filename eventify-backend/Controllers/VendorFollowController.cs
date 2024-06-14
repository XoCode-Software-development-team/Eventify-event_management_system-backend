using eventify_backend.Data;
using eventify_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eventify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorFollowController : ControllerBase
    {
        private readonly VendorFollowService _vendorFollowService;

        public VendorFollowController(VendorFollowService vendorFollowService)
        {
            _vendorFollowService = vendorFollowService;
        }

        [HttpGet("isFollow/{soRId}")]
        [Authorize]
        public async Task<IActionResult> IsFollow(int soRId)
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

                var isFollow = await _vendorFollowService.IsFollowAsync(soRId, userId);

                return Ok(new { IsFollow = isFollow });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new {Message = ex.Message});
            }
        }


        [HttpPut("isFollow/{soRId}")]
        [Authorize]
        public async Task<IActionResult> ToggleFollow(int soRId)
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

                var Message = await _vendorFollowService.ToggleFollowAsync(soRId, userId);

                return Ok(new { Message = Message });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
