using System.Collections.Concurrent;

namespace AegisLink.Server.Services;

public sealed class HubSessionTracker
{
    // Maps connectionId → set of sessionIds the connection has joined.
    // ConcurrentDictionary<string, byte> is used as a thread-safe set (no locking required).
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _connectionSessions = new();

    public void Join(string connectionId, string sessionId)
    {
        var sessions = _connectionSessions.GetOrAdd(connectionId, _ => new ConcurrentDictionary<string, byte>());
        sessions.TryAdd(sessionId, 0);
    }

    public bool IsMember(string connectionId, string sessionId)
    {
        return _connectionSessions.TryGetValue(connectionId, out var sessions)
            && sessions.ContainsKey(sessionId);
    }

    public void Remove(string connectionId)
    {
        _connectionSessions.TryRemove(connectionId, out _);
    }
}
