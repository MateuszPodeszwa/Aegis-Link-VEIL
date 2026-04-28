using Microsoft.AspNetCore.SignalR.Client;

/// <summary>
/// Wraps SignalR hub communication for real-time encrypted chat events.
/// </summary>
/// <remarks>
/// This service is responsible for connection lifecycle, session join/rejoin behaviour,
/// and pushing hub events back to UI code through C# events.
/// </remarks>
public class ChatHubService : IAsyncDisposable
{
    // Underlying SignalR connection used for all chat hub calls.
    private HubConnection? _connection;
    // We store the session id so reconnect can auto-join the same room.
    private string? _sessionId;

    /// <summary>
    /// Raised when a new encrypted message arrives and has been decrypted.
    /// </summary>
    public event Func<string, Task>? OnReceiveMessage;
    /// <summary>
    /// Raised when the server confirms a message was deleted.
    /// </summary>
    public event Func<Guid, Task>? OnMessageDeleted;
    /// <summary>
    /// Raised when another user joins the current chat session.
    /// </summary>
    public event Func<Task>? OnUserJoined;
    /// <summary>
    /// Raised when connection status text should be updated in the UI.
    /// </summary>
    public event Func<string, Task>? OnStatusChanged;

    /// <summary>
    /// Current SignalR connection state, or <c>null</c> if not initialised.
    /// </summary>
    public HubConnectionState? State => _connection?.State;

    /// <summary>
    /// Creates and starts the hub connection, then joins the requested session.
    /// </summary>
    /// <param name="hubUrl">Absolute or relative hub URL.</param>
    /// <param name="decryptFn">Function used to decrypt received payloads.</param>
    /// <param name="encryptFn">Function used for encryption (reserved for future use in this service).</param>
    /// <param name="sharedKey">Shared key used by decrypt function.</param>
    /// <param name="sessionId">Session id to join.</param>
    public async Task InitializeAsync(
        string hubUrl,
        Func<string, string, Task<string>> decryptFn,
        Func<string, string, Task<string>> encryptFn,
        string sharedKey,
        string sessionId)
    {
        // Keep the current session id for reconnect handling.
        _sessionId = sessionId;

        // Build one SignalR connection with automatic reconnect enabled.
        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _connection.Reconnecting += async _ =>
        {
            // Tell the UI we are trying to restore connectivity.
            if (OnStatusChanged != null)
                await OnStatusChanged.Invoke("Reconnecting...");
        };

        _connection.Reconnected += async _ =>
        {
            // Rejoin the room after reconnect so message events keep working.
            if (_sessionId is not null)
                await _connection.SendAsync("JoinSession", _sessionId);

            // If reconnect worked, mark status as connected again.
            if (OnStatusChanged != null)
                await OnStatusChanged.Invoke("Connected");
        };

        _connection.Closed += async _ =>
        {
            // If connection fully closes, reflect that in UI status.
            if (OnStatusChanged != null)
                await OnStatusChanged.Invoke("Disconnected");
        };

        _connection.On<string>("ReceiveMessage", async payload =>
        {
            // Payload arrives encrypted, so we decrypt before raising the event.
            var decryptedJson = await decryptFn(payload, sharedKey);

            if (OnReceiveMessage != null)
                await OnReceiveMessage.Invoke(decryptedJson);
        });

        _connection.On<Guid>("MessageDeleted", async id =>
        {
            // Forward deletion notifications to page state.
            if (OnMessageDeleted != null)
                await OnMessageDeleted.Invoke(id);
        });

        _connection.On("UserJoined", async () =>
        {
            // Let UI know a participant joined this session.
            if (OnUserJoined != null)
                await OnUserJoined.Invoke();
        });

        // Start SignalR transport and register handlers.
        await _connection.StartAsync();

        // Join once after first connect as well.
        if (_sessionId is not null)
            await _connection.SendAsync("JoinSession", _sessionId);

        if (OnStatusChanged != null)
            await OnStatusChanged.Invoke("Connected");
    }

    /// <summary>
    /// Sends an already encrypted message payload to the current session.
    /// </summary>
    /// <param name="sessionId">Target session id.</param>
    /// <param name="cipher">Encrypted message payload.</param>
    public async Task SendMessage(string sessionId, string cipher)
    {
        // Safety check: do nothing if the connection is not currently online.
        if (_connection?.State != HubConnectionState.Connected) return;

        await _connection.SendAsync("SendMessage", sessionId, cipher);
    }

    /// <summary>
    /// Requests deletion of a message by id for a given session.
    /// </summary>
    /// <param name="sessionId">Target session id.</param>
    /// <param name="messageId">Unique message id to remove.</param>
    public async Task DeleteMessage(string sessionId, Guid messageId)
    {
        // Safety check: avoid hub calls if connection is offline.
        if (_connection?.State != HubConnectionState.Connected) return;

        await _connection.SendAsync("DeleteMessage", sessionId, messageId);
    }

    /// <summary>
    /// Disposes the hub connection and releases SignalR resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        // Dispose only when connection was created.
        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}
