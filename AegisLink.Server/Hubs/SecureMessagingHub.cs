using Microsoft.AspNetCore.SignalR;

namespace AegisLink.Server.Hubs;

public class SecureMessagingHub : Hub
{
    // Users join a specific Aegis Link session (room)
    public async Task JoinSession(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
            
        // Notify the other user in the room that a partner connected
        await Clients.GroupExcept(sessionId, Context.ConnectionId).SendAsync("UserJoined");
    }

    // The server acts as a blind relay for the messages
    public async Task SendMessage(string sessionId, string payload)
    {
        // Send the payload to everyone in the room EXCEPT the sender
        await Clients.GroupExcept(sessionId, Context.ConnectionId).SendAsync("ReceiveMessage", payload);
    }
}