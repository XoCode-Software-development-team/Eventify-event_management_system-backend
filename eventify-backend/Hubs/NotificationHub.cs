using Microsoft.AspNetCore.SignalR;
using eventify_backend.Services;
using System;
using System.Threading.Tasks;

namespace eventify_backend.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly NotificationService _notificationService;

        public NotificationHub(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task SendNotificationCount(Guid userId)
        {
            int unreadCount = await _notificationService.GetUnreadNotificationCount(userId);
            await Clients.User(userId.ToString()).SendAsync("ReceiveNotificationCount", unreadCount);
        }

        public async Task SendNotification(Guid userId, string message)
        {
            await Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
        }
    }
}
