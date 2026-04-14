using Microsoft.AspNetCore.SignalR.Client;

public class ChatHubService : IAsyncDisposable
{
    private HubConnection? _connection;

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
            if (OnStatusChanged != null)
                await OnStatusChanged.Invoke("Connected");
        };

        _connection.On<string>("ReceiveMessage", async payload =>
        {
            var decryptedJson = await decryptFn(payload, sharedKey);
            OnReceiveMessage?.Invoke(decryptedJson);
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
        await _connection.SendAsync("JoinSession", sessionId);

        if (OnStatusChanged != null)
            await OnStatusChanged.Invoke("Connected");
    }

    public async Task SendMessage(string sessionId, string cipher)
    {
        if (_connection is null) return;
        await _connection.SendAsync("SendMessage", sessionId, cipher);
    }

    public async Task DeleteMessage(string sessionId, Guid messageId)
    {
        if (_connection is null) return;
        await _connection.SendAsync("DeleteMessage", sessionId, messageId);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}