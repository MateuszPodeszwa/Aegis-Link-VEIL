using System.Collections.Concurrent;
using AegisLink.Tests.TestUtilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace AegisLink.Tests.Client;

public class ChatHubServiceTests
{
    [Fact]
    public async Task InitializeAsync_ReceivesHubEvents()
    {
        await using var host = await TestChatHost.StartAsync();
        var service = new ChatHubService();
        var received = new TaskCompletionSource<string>();
        var deleted = new TaskCompletionSource<Guid>();
        var joined = new TaskCompletionSource<bool>();
        var statuses = new ConcurrentQueue<string>();

        service.OnReceiveMessage += message =>
        {
            received.TrySetResult(message);
            return Task.CompletedTask;
        };
        service.OnMessageDeleted += messageId =>
        {
            deleted.TrySetResult(messageId);
            return Task.CompletedTask;
        };
        service.OnUserJoined += () =>
        {
            joined.TrySetResult(true);
            return Task.CompletedTask;
        };
        service.OnStatusChanged += status =>
        {
            statuses.Enqueue(status);
            return Task.CompletedTask;
        };

        await service.InitializeAsync(
            host.HubUrl,
            (payload, _) => Task.FromResult($"decrypted:{payload}"),
            (_, _) => Task.FromResult("ignored"),
            "shared",
            "session-1");

        await service.SendMessage("session-1", "cipher");
        await service.DeleteMessage("session-1", Guid.NewGuid());

        var message = await WaitFor(received.Task);
        var deletedId = await WaitFor(deleted.Task);
        var joinedEvent = await WaitFor(joined.Task);

        Assert.Equal("decrypted:cipher", message);
        Assert.NotEqual(Guid.Empty, deletedId);
        Assert.True(joinedEvent);
        Assert.Contains("Connected", statuses);

        await service.DisposeAsync();
    }

    [Fact]
    public async Task SendMessage_IgnoresWhenDisconnected()
    {
        var service = new ChatHubService();

        await service.SendMessage("session-1", "cipher");

        await service.DisposeAsync();
    }

    private static async Task<T> WaitFor<T>(Task<T> task)
    {
        var completed = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(5)));
        Assert.Same(task, completed);
        return await task;
    }

    private sealed class TestChatHub : Hub
    {
        public async Task JoinSession(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
            await Clients.Group(sessionId).SendAsync("UserJoined");
        }

        public Task SendMessage(string sessionId, string payload)
        {
            return Clients.Group(sessionId).SendAsync("ReceiveMessage", payload);
        }

        public Task DeleteMessage(string sessionId, Guid messageId)
        {
            return Clients.Group(sessionId).SendAsync("MessageDeleted", messageId);
        }
    }

    private sealed class TestChatHost : IAsyncDisposable
    {
        private readonly WebApplication _app;
        public string HubUrl { get; }

        private TestChatHost(WebApplication app, string hubUrl)
        {
            _app = app;
            HubUrl = hubUrl;
        }

        public static async Task<TestChatHost> StartAsync()
        {
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.UseUrls("http://127.0.0.1:0");
            builder.Services.AddSignalR();

            var app = builder.Build();
            app.MapHub<TestChatHub>("/chatHub");
            await app.StartAsync();

            var server = app.Services.GetRequiredService<IServer>();
            var feature = server.Features.Get<IServerAddressesFeature>();
            var address = feature?.Addresses.First();
            if (address is null)
            {
                throw new InvalidOperationException("Failed to resolve test server address.");
            }

            return new TestChatHost(app, $"{address.TrimEnd('/')}/chatHub");
        }

        public async ValueTask DisposeAsync()
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
    }
}
