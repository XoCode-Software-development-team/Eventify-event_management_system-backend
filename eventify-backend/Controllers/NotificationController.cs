using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using eventify_backend.Models;
using eventify_backend.Services;
using Microsoft.AspNetCore.Authorization;

namespace eventify_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationsController(NotificationService notificationService, IHubContext<NotificationHub> hubContext)
        {
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetNotifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Extract userId from the JWT token
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null)
            {
                return Unauthorized(new { Message = "User ID is missing in the token." });
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var notifications = await _notificationService.GetNotificationsAsync(userId, pageNumber, pageSize);
            if (notifications == null)
            {
                return NotFound();
            }
            return Ok(notifications);
        }

        [HttpPut("{notificationId}")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            // Extract userId from the JWT token
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null)
            {
                return Unauthorized(new { Message = "User ID is missing in the token." });
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var notification = await _notificationService.MarkAsReadAsync(userId, notificationId);
            if (notification == null)
            {
                return NotFound();
            }

            return Ok(notification);
        }


        [HttpPut("markAllRead")]
        [Authorize]
        public async Task<IActionResult> MarkAllAsRead()
        {
            // Extract userId from the JWT token
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null)
            {
                return Unauthorized(new { Message = "User ID is missing in the token." });
            }

            var userId = Guid.Parse(userIdClaim.Value);

            await _notificationService.MarkAllAsReadAsync(userId);

            return NoContent();
        }

        [HttpDelete("deleteAll")]
        [Authorize]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            // Extract userId from the JWT token
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null)
            {
                return Unauthorized(new { Message = "User ID is missing in the token." });
            }

            var userId = Guid.Parse(userIdClaim.Value);

            await _notificationService.DeleteAllNotificationsAsync(userId);

            return Ok();
        }

        [HttpDelete("delete/{notificationId}")]
        [Authorize]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            // Extract userId from the JWT token
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null)
            {
                return Unauthorized(new { Message = "User ID is missing in the token." });
            }

            var userId = Guid.Parse(userIdClaim.Value);

            await _notificationService.DeleteNotificationAsync(userId,notificationId);

            return Ok();
        }
    }
}
