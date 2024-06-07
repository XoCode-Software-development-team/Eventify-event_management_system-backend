using eventify_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eventify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly UserProfileService _userProfileService;

        public UserProfileController(UserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }
        [HttpGet("Avatar")]
        [Authorize]
        public async Task<IActionResult> GetUserAvatar()
        {
            try
            {
                // Extract vendorId from the JWT token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Message = "User ID is missing in the token." });
                }

                var userId = Guid.Parse(userIdClaim.Value);

                var userImage = await _userProfileService.GetUserAvatarAsync(userId);

                if (userImage == null)
                {
                    return NotFound(new
                    {
                        Message = "User image not found"
                    });
                }

                return Ok(new
                {
                    Message = "Image load successfull!",
                    userImage = userImage
                });
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");

            }
        }
    }
}
