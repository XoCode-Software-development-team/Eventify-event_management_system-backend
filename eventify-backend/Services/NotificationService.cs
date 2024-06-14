using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eventify_backend.Data;
using eventify_backend.DTOs;
using eventify_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using eventify_backend.Hubs;

namespace eventify_backend.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(AppDbContext appDbContext, IHubContext<NotificationHub> hubContext)
        {
            _appDbContext = appDbContext;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<NotificationDTO>> GetNotificationsAsync(Guid userId, int pageNumber, int pageSize)
        {
            // Ensure pageNumber and pageSize are valid
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var notifications = await _appDbContext.Notification
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.TimeStamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    Message = n.Message,
                    TimeStamp = n.TimeStamp,
                    Read = n.Read
                })
                .ToListAsync();

            return notifications;
        }

        public async Task<int> GetUnreadNotificationCount(Guid userId)
        {
            // Count the unread notifications for the user
            var unreadCount = await _appDbContext.Notification
                .Where(n => n.UserId == userId && !n.Read)
                .CountAsync();

            return unreadCount;
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var notifications = await _appDbContext.Notification
                .Where(n => n.UserId == userId && !n.Read)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.Read = true;
            }

            await _appDbContext.SaveChangesAsync();

            // Send real-time update to clients
            int unreadCount = await GetUnreadNotificationCount(userId);
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotificationCount", unreadCount);
        }

        public async Task AddUpdatedNotificationAsync(Guid vendorId, int soRId)
        {
            // Fetch all followers of the vendor
            var clients = await _appDbContext.VendorFollows
                .Where(f => f.VendorId == vendorId)
                .ToListAsync();

            // Fetch the company name of the vendor
            var companyName = await _appDbContext.Vendors
                .Where(v => v.UserId == vendorId)
                .Select(v => v.CompanyName)
                .FirstOrDefaultAsync();

            // Fetch the name of the service or resource
            var serviceResourceName = await _appDbContext.ServiceAndResources
                .Where(s => s.SoRId == soRId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();

            // Check if the entity is a service
            var isService = await _appDbContext.Services
                .AnyAsync(s => s.SoRId == soRId);

            // Create a list to hold all created notifications
            var notifications = new List<Notification>();

            // Iterate through each follower and create a notification for them
            foreach (var client in clients)
            {
                var notification = new Notification
                {
                    UserId = client.ClientId,
                    Message = $"The {companyName} has updated the {(isService ? "service" : "resource")} {serviceResourceName} details. Visit and check it out.",
                    TimeStamp = DateTime.Now,
                    Read = false,
                };

                notifications.Add(notification);
            }

            // Add notifications to the database
            _appDbContext.Notification.AddRange(notifications);
            await _appDbContext.SaveChangesAsync();

            // Send real-time update to each client
            foreach (var notification in notifications)
            {
                int unreadCount = await GetUnreadNotificationCount(notification.UserId);

                await _hubContext.Clients.User(notification.UserId.ToString())
                    .SendAsync("ReceiveNotification", notification);

                await _hubContext.Clients.User(notification.UserId.ToString())
                    .SendAsync("ReceiveNotificationCount", unreadCount);
            }
        }

        public async Task<Notification?> MarkAsReadAsync(Guid userId, int notificationId)
        {
            var notification = await _appDbContext.Notification
                .FirstOrDefaultAsync(n => n.UserId == userId && n.NotificationId == notificationId);

            if (notification != null)
            {
                notification.Read = true;
                await _appDbContext.SaveChangesAsync();

                // Send real-time update to clients
                int unreadCount = await GetUnreadNotificationCount(userId);
                await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotificationCount", unreadCount);
            }

            return notification;
        }

        public async Task DeleteAllNotificationsAsync(Guid userId)
        {
            var notifications = await _appDbContext.Notification
                .Where(n => n.UserId == userId)
                .ToListAsync();

            if (notifications != null && notifications.Any())
            {
                _appDbContext.RemoveRange(notifications);
                await _appDbContext.SaveChangesAsync();

                // Send real-time update to clients
                int unreadCount = await GetUnreadNotificationCount(userId);
                await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotificationCount", unreadCount);
            }
        }

        public async Task DeleteNotificationAsync(Guid userId, int notificationId)
        {
            var notification = await _appDbContext.Notification
                .FirstOrDefaultAsync(n => n.UserId == userId && n.NotificationId == notificationId);

            if (notification != null)
            {
                _appDbContext.Notification.Remove(notification);
                await _appDbContext.SaveChangesAsync();

                // Send real-time update to clients
                int unreadCount = await GetUnreadNotificationCount(userId);
                await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotificationCount", unreadCount);
            }
        }
    }
}
