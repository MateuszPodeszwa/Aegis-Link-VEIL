using Microsoft.AspNetCore.SignalR.Client;

public class ChatHubService : IAsyncDisposable
{
    private HubConnection? _connection;
    private string? _sessionId;

    public event Func<string, Task>? OnReceiveMessage;
    public event Func<Guid, Task>? OnMessageDeleted;
    public event Func<Task>? OnUserJoined;
    public event Func<string, Task>? OnStatusChanged;

    public HubConnectionState? State => _connection?.State;

    public async Task InitializeAsync(
        string hubUrl,
        Func<string, string, Task<string>> decryptFn,
        Func<string, string, Task<string>> encryptFn,
        string sharedKey,
        string sessionId)
    {
        _sessionId = sessionId;

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _connection.Reconnecting += async _ =>
        {
            if (OnStatusChanged != null)
                await OnStatusChanged.Invoke("Reconnecting...");
        };

        _connection.Reconnected += async _ =>
        {
            if (_sessionId is not null)
                await _connection.SendAsync("JoinSession", _sessionId);

            if (OnStatusChanged != null)
                await OnStatusChanged.Invoke("Connected");
        };

        _connection.Closed += async _ =>
        {
            if (OnStatusChanged != null)
                await OnStatusChanged.Invoke("Disconnected");
        };

        _connection.On<string>("ReceiveMessage", async payload =>
        {
            var decryptedJson = await decryptFn(payload, sharedKey);

            if (OnReceiveMessage != null)
                await OnReceiveMessage.Invoke(decryptedJson);
        });

        _connection.On<Guid>("MessageDeleted", async id =>
        {
            if (OnMessageDeleted != null)
                await OnMessageDeleted.Invoke(id);
        });

        _connection.On("UserJoined", async () =>
        {
            if (OnUserJoined != null)
                await OnUserJoined.Invoke();
        });

        await _connection.StartAsync();

        if (_sessionId is not null)
            await _connection.SendAsync("JoinSession", _sessionId);

        if (OnStatusChanged != null)
            await OnStatusChanged.Invoke("Connected");
    }

    public async Task SendMessage(string sessionId, string cipher)
    {
        if (_connection?.State != HubConnectionState.Connected) return;

        await _connection.SendAsync("SendMessage", sessionId, cipher);
    }

    public async Task DeleteMessage(string sessionId, Guid messageId)
    {
        if (_connection?.State != HubConnectionState.Connected) return;

        await _connection.SendAsync("DeleteMessage", sessionId, messageId);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}