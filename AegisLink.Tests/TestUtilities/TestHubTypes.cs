using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Features;

namespace AegisLink.Tests.TestUtilities;

public sealed class TestClientProxy : IClientProxy
{
    public List<(string Method, object?[]? Args)> Sent { get; } = new();

    public Task SendCoreAsync(string method, object?[]? args, CancellationToken cancellationToken = default)
    {
        Sent.Add((method, args));
        return Task.CompletedTask;
    }
}

public sealed class TestHubCallerClients : IHubCallerClients
{
    public TestClientProxy GroupExceptProxy { get; } = new();

    public IClientProxy All => throw new NotImplementedException();
    public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => throw new NotImplementedException();
    public IClientProxy Client(string connectionId) => throw new NotImplementedException();
    public IClientProxy Clients(IReadOnlyList<string> connectionIds) => throw new NotImplementedException();
    public IClientProxy Group(string groupName) => throw new NotImplementedException();
    public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => GroupExceptProxy;
    public IClientProxy Groups(IReadOnlyList<string> groupNames) => throw new NotImplementedException();
    public IClientProxy User(string userId) => throw new NotImplementedException();
    public IClientProxy Users(IReadOnlyList<string> userIds) => throw new NotImplementedException();
    public IClientProxy Caller => throw new NotImplementedException();
    public IClientProxy Others => throw new NotImplementedException();
    public IClientProxy OthersInGroup(string groupName) => throw new NotImplementedException();
}

public sealed class TestGroupManager : IGroupManager
{
    public List<(string ConnectionId, string GroupName)> Added { get; } = new();

    public Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
    {
        Added.Add((connectionId, groupName));
        return Task.CompletedTask;
    }

    public Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

public sealed class TestHubCallerContext : HubCallerContext
{
    public TestHubCallerContext(string connectionId)
    {
        ConnectionId = connectionId;
    }

    public override string ConnectionId { get; }
    public override string? UserIdentifier => null;
    public override ClaimsPrincipal? User => null;
    public override IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();
    public override IFeatureCollection Features { get; } = new FeatureCollection();
    public override CancellationToken ConnectionAborted => CancellationToken.None;
    public override void Abort()
    {
    }
}
