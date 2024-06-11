using eventify_backend.DTOs;
using eventify_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpPut("updateAvatar")]
        [Authorize]
        public async Task<IActionResult> UpdateAvatar([FromBody] string[] imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl[0]))
                    return BadRequest(new { Message = "Image URL is empty!" });

                // Extract userId from the JWT token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Message = "User ID is missing in the token." });
                }

                var userId = Guid.Parse(userIdClaim.Value);

                // Call the service method to update the avatar
                var result = await _userProfileService.UpdateAvatarAsync(userId, imageUrl[0]);

                if (result)
                {
                    return Ok(new { Message = "Avatar updated successfully!" });
                }
                else
                {
                    return StatusCode(500, new { Message = "Failed to update picture." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpDelete("deleteAvatar")]
        [Authorize]
        public async Task<IActionResult> DeleteUserImage()
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

                var imageUrl = await _userProfileService.DeleteUserImageAsync(userId);

                if (imageUrl == null)
                {
                    return BadRequest(new { Message = "Avatar not found!" });
                }
                else
                {
                    return Ok(new { Message = "Avatar deleted!", imageUrl = imageUrl });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("userDetails")]
        [Authorize]
        public async Task<IActionResult> GetUserDetails()
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

                var userDetails = await _userProfileService.GetUserDetailsAsync(userId);

                if (userDetails == null)
                {
                    return BadRequest(new { Message = "Invalid User!" });
                }

                return Ok(new { Message = "User details found!", UserDetails = userDetails });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPut("userDetails")]
        [Authorize]
        public async Task<IActionResult> UpdateUserDetail([FromBody] UserDetailsDTO userDetails)
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

                var userRoleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                if (userRoleClaim == null)
                {
                    return Unauthorized(new { Message = "User role is missing in the token." });
                }

                var userRole = userRoleClaim.Value;

                var result = await _userProfileService.UpdateUserDetailsAsync(userId, userRole, userDetails);

                if (result)
                {
                    return Ok(new { Message = "User details updated successfully!" });
                }
                else { return BadRequest(new { Message = "User details updated faild!" }); }

            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteUser()
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

                var result = await _userProfileService.DeleteUserAsync(userId);

                if (result)
                {
                    return Ok(new { Message = "User delete successfull!" });
                }

                return BadRequest(new { Message = "User delete failed!" });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });

            }
        }

    }
}
