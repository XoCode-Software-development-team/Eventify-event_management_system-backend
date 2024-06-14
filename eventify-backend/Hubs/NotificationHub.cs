using eventify_backend.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace eventify_backend.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly NotificationService _notificationService;
        private static ConcurrentDictionary<Guid, string> userConnectionMap = new ConcurrentDictionary<Guid, string>();

        public NotificationHub(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public override async Task OnConnectedAsync()
        {
            // The user ID should be sent as a query string parameter when connecting
            var userIdString = Context.GetHttpContext()?.Request.Query["userId"];
            if (Guid.TryParse(userIdString, out Guid userId))
            {
                userConnectionMap[userId] = Context.ConnectionId;
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserIdFromConnectionId(Context.ConnectionId);
            if (userId != null)
            {
                userConnectionMap.TryRemove((Guid)userId, out _);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public Guid? GetUserIdFromConnectionId(string connectionId)
        {
            foreach (var kvp in userConnectionMap)
            {
                if (kvp.Value == connectionId)
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        public string? GetConnectionIdFromUserId (Guid userId)
        {
            foreach (var kvp in userConnectionMap)
            {
                if (kvp.Key == userId)
                {
                    return kvp.Value;
                }
            }
            return null;
        }

        public async Task SendNotificationCount(Guid userId)
        {
            int unreadCount = await _notificationService.GetUnreadNotificationCount(userId);
            if (userConnectionMap.TryGetValue(userId, out string? connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotificationCount", unreadCount);
            }
        }

        public async Task SendNotification(Guid userId, string message)
        {
            if (userConnectionMap.TryGetValue(userId, out string? connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
            }
        }
    }
}
