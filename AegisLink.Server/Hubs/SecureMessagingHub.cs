using System.Text.RegularExpressions;
using AegisLink.Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace AegisLink.Server.Hubs;

public class SecureMessagingHub : Hub
{
    // Room IDs are 32 lowercase hex chars (16-byte SHA-256 prefix from computeRoomId).
    private static readonly Regex SessionIdPattern = new(@"^[0-9a-f]{32}$", RegexOptions.Compiled);

    private readonly HubSessionTracker _tracker;
    private readonly ILogger<SecureMessagingHub> _logger;

    public SecureMessagingHub(HubSessionTracker tracker, ILogger<SecureMessagingHub> logger)
    {
        _tracker = tracker;
        _logger = logger;
    }

    // Users join a specific session room identified by the deterministic room ID.
    public async Task JoinSession(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId) || !SessionIdPattern.IsMatch(sessionId))
        {
            _logger.LogWarning("JoinSession rejected: invalid sessionId format from connection {ConnectionId}.", Context.ConnectionId);
            return;
        }

        _tracker.Join(Context.ConnectionId, sessionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        await Clients.GroupExcept(sessionId, Context.ConnectionId).SendAsync("UserJoined");
    }

    // Blind relay — forwards ciphertext to all other participants in the session.
    // Only relays if the sending connection previously joined the session.
    public async Task SendMessage(string sessionId, string payload)
    {
        if (string.IsNullOrEmpty(sessionId) || !SessionIdPattern.IsMatch(sessionId))
        {
            _logger.LogWarning("SendMessage rejected: invalid sessionId format from connection {ConnectionId}.", Context.ConnectionId);
            return;
        }

        if (!_tracker.IsMember(Context.ConnectionId, sessionId))
        {
            _logger.LogWarning("SendMessage rejected: connection {ConnectionId} is not a member of session {SessionId}.", Context.ConnectionId, sessionId);
            return;
        }

        await Clients.GroupExcept(sessionId, Context.ConnectionId).SendAsync("ReceiveMessage", payload);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _tracker.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
