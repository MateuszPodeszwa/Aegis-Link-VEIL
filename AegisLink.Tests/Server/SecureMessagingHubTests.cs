using AegisLink.Server.Hubs;
using AegisLink.Server.Services;
using AegisLink.Tests.TestUtilities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging.Abstractions;

namespace AegisLink.Tests.Server;

public class SecureMessagingHubTests
{
    [Fact]
    public async Task JoinSession_RejectsInvalidId()
    {
        var tracker = new HubSessionTracker();
        var hub = CreateHub(tracker, out var clients, out var groups);

        await Assert.ThrowsAsync<HubException>(() => hub.JoinSession("bad"));

        Assert.Empty(groups.Added);
        Assert.False(tracker.IsMember("conn-1", "bad"));
        Assert.Empty(clients.GroupExceptProxy.Sent);
    }

    [Fact]
    public async Task JoinSession_AddsToGroupAndNotifies()
    {
        var tracker = new HubSessionTracker();
        var hub = CreateHub(tracker, out var clients, out var groups);
        var sessionId = new string('a', 32);

        await hub.JoinSession(sessionId);

        Assert.True(tracker.IsMember("conn-1", sessionId));
        Assert.Single(groups.Added);
        Assert.Equal(("conn-1", sessionId), groups.Added[0]);
        Assert.Single(clients.GroupExceptProxy.Sent);
        Assert.Equal("UserJoined", clients.GroupExceptProxy.Sent[0].Method);
    }

    [Fact]
    public async Task SendMessage_RejectsInvalidId()
    {
        var tracker = new HubSessionTracker();
        var hub = CreateHub(tracker, out _, out _);

        await Assert.ThrowsAsync<HubException>(() => hub.SendMessage("bad", "payload"));
    }

    [Fact]
    public async Task SendMessage_RejectsNonMember()
    {
        var tracker = new HubSessionTracker();
        var hub = CreateHub(tracker, out _, out _);
        var sessionId = new string('a', 32);

        await Assert.ThrowsAsync<HubException>(() => hub.SendMessage(sessionId, "payload"));
    }

    [Fact]
    public async Task SendMessage_ForwardsToGroup()
    {
        var tracker = new HubSessionTracker();
        var hub = CreateHub(tracker, out var clients, out _);
        var sessionId = new string('a', 32);
        tracker.Join("conn-1", sessionId);

        await hub.SendMessage(sessionId, "cipher");

        Assert.Single(clients.GroupExceptProxy.Sent);
        Assert.Equal("ReceiveMessage", clients.GroupExceptProxy.Sent[0].Method);
        Assert.Equal("cipher", clients.GroupExceptProxy.Sent[0].Args?[0]);
    }

    [Fact]
    public async Task DeleteMessage_ForwardsToGroup()
    {
        var tracker = new HubSessionTracker();
        var hub = CreateHub(tracker, out var clients, out _);
        var sessionId = new string('a', 32);
        var messageId = Guid.NewGuid();
        tracker.Join("conn-1", sessionId);

        await hub.DeleteMessage(sessionId, messageId);

        Assert.Single(clients.GroupExceptProxy.Sent);
        Assert.Equal("MessageDeleted", clients.GroupExceptProxy.Sent[0].Method);
        Assert.Equal(messageId, clients.GroupExceptProxy.Sent[0].Args?[0]);
    }

    [Fact]
    public async Task DeleteMessage_RejectsNonMember()
    {
        var tracker = new HubSessionTracker();
        var hub = CreateHub(tracker, out _, out _);
        var sessionId = new string('a', 32);

        await Assert.ThrowsAsync<HubException>(() => hub.DeleteMessage(sessionId, Guid.NewGuid()));
    }

    [Fact]
    public async Task OnDisconnected_RemovesTracking()
    {
        var tracker = new HubSessionTracker();
        var hub = CreateHub(tracker, out _, out _);
        var sessionId = new string('a', 32);
        tracker.Join("conn-1", sessionId);

        await hub.OnDisconnectedAsync(null);

        Assert.False(tracker.IsMember("conn-1", sessionId));
    }

    private static SecureMessagingHub CreateHub(
        HubSessionTracker tracker,
        out TestHubCallerClients clients,
        out TestGroupManager groups)
    {
        clients = new TestHubCallerClients();
        groups = new TestGroupManager();
        return new SecureMessagingHub(tracker, NullLogger<SecureMessagingHub>.Instance)
        {
            Clients = clients,
            Groups = groups,
            Context = new TestHubCallerContext("conn-1")
        };
    }
}
