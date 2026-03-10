using Microsoft.AspNetCore.SignalR;

namespace AegisLink.Server.Hubs;

public class SecureMessagingHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}