using Microsoft.AspNetCore.SignalR;

namespace AegisLink.Server.Hubs;

public class SecureMessagingHub : Hub
{
    // Users join a specific session room identified by the deterministic room ID.
    public async Task JoinSession(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        await Clients.GroupExcept(sessionId, Context.ConnectionId).SendAsync("UserJoined");
    }

    // Blind relay — forwards ciphertext to all other participants in the session.
    public async Task SendMessage(string sessionId, string payload)
    {
        await Clients.GroupExcept(sessionId, Context.ConnectionId).SendAsync("ReceiveMessage", payload);
    }
}
