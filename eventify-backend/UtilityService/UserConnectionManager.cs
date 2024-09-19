using System.Collections.Concurrent;

public class UserConnectionManager
{
    public ConcurrentDictionary<Guid, string> UserConnectionMap { get; } = new ConcurrentDictionary<Guid, string>();

    public void AddConnection(Guid userId, string connectionId)
    {
        UserConnectionMap[userId] = connectionId;
    }

    public void RemoveConnection(string connectionId)
    {
        var item = UserConnectionMap.FirstOrDefault(kvp => kvp.Value == connectionId);
        if (!item.Equals(default(KeyValuePair<Guid, string>)))
        {
            UserConnectionMap.TryRemove(item.Key, out _);
        }
    }

    public string? GetConnectionId(Guid userId)
    {
        UserConnectionMap.TryGetValue(userId, out string? connectionId);
        return connectionId;
    }
}
