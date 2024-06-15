using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
    private readonly NotificationService _notificationService;
    private readonly UserConnectionManager _userConnectionManager;

    public NotificationHub(NotificationService notificationService, UserConnectionManager userConnectionManager)
    {
        _notificationService = notificationService;
        _userConnectionManager = userConnectionManager;
    }

    public override async Task OnConnectedAsync()
    {
        var userIdString = Context.GetHttpContext()?.Request.Query["userId"];
        if (Guid.TryParse(userIdString, out Guid userId))
        {
            _userConnectionManager.AddConnection(userId, Context.ConnectionId);
            await SendNotificationCount(userId); // Send unread count on connection
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _userConnectionManager.RemoveConnection(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendNotificationCount(Guid userId)
    {
        int unreadCount = await _notificationService.GetUnreadNotificationCount(userId);
        var connectionId = _userConnectionManager.GetConnectionId(userId);
        if (connectionId != null)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveNotificationCount", unreadCount);
        }
    }

    public async Task SendNotification(Guid userId, string message)
    {
        var connectionId = _userConnectionManager.GetConnectionId(userId);
        if (connectionId != null)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
        }
    }
}
