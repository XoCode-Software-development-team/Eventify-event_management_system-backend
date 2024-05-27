using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using eventify_backend.Hubs;
using eventify_backend.Models;
using eventify_backend.Services;

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

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetNotifications(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var notifications = await _notificationService.GetNotificationsAsync(userId, pageNumber, pageSize);
            if (notifications == null)
            {
                return NotFound();
            }
            return Ok(notifications);
        }

        [HttpPut("/api/[Controller]/{userId}/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(Guid userId, int notificationId)
        {
            var notification = await _notificationService.MarkAsReadAsync(userId, notificationId);
            if (notification == null)
            {
                return NotFound();
            }

            // Trigger SignalR notification
            await _hubContext.Clients.All.SendAsync("NotificationMarkedAsRead", notificationId);

            return Ok(notification);
        }


        [HttpPut("markAllRead/{userId}")]
        public async Task<IActionResult> MarkAllAsRead(Guid userId)
        {
            await _notificationService.MarkAllAsReadAsync(userId);

            // Trigger SignalR notification
            await _hubContext.Clients.All.SendAsync("AllNotificationsMarkedAsRead");

            return NoContent();
        }

        [HttpDelete("deleteAll/{userId}")]
        public async Task<IActionResult> DeleteAllNotifications(Guid userId)
        {
            await _notificationService.DeleteAllNotificationsAsync(userId);

            return Ok();
        }

        [HttpDelete("delete/{userId}/{notificationId}")]
        public async Task<IActionResult> DeleteNotification(Guid userId,int notificationId)
        {
            await _notificationService.DeleteNotificationAsync(userId,notificationId);

            return Ok();
        }
    }
}
