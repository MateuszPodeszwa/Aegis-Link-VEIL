using Microsoft.JSInterop;
using System.Text.Json;

/// <summary>
/// Stores chat sessions in memory and syncs them with browser session storage.
/// </summary>
/// <remarks>
/// This keeps the chat list alive while the tab is open and avoids repeated JS fetch work.
/// </remarks>
public class ChatStore
{
    // JS runtime bridge used to read/write from browser storage.
    private readonly IJSRuntime _js;
    // We keep all chat sessions in browser session storage with this key.
    private const string StorageKey = "aegis_chats";
    // This makes sure setup only runs once, even if called many times.
    private Task? _initTask;

    // In-memory list used by pages while the app is open.
    public List<ChatSession> Chats { get; private set; } = new();

    /// <summary>
    /// Creates a new chat store.
    /// </summary>
    /// <param name="js">JavaScript runtime used for browser interop calls.</param>
    public ChatStore(IJSRuntime js)
    {
        _js = js;
    }

    /// <summary>
    /// Initialises the chat cache once per service lifetime.
    /// </summary>
    /// <returns>A task representing initial load from browser storage.</returns>
    public Task InitializeAsync()
    {
        // Cache the startup task so we do not load the same data twice.
        return _initTask ??= InitializeCoreAsync();
    }

    /// <summary>
    /// Loads chat sessions from browser storage.
    /// </summary>
    private async Task InitializeCoreAsync()
    {
        // Pull stored JSON text from session storage.
        var json = await _js.InvokeAsync<string>("sessionStorage.getItem", StorageKey);

        if (!string.IsNullOrEmpty(json))
        {
            // If deserialising fails, we fall back to an empty list.
            Chats = JsonSerializer.Deserialize<List<ChatSession>>(json)
                    ?? new List<ChatSession>();
        }
    }

    /// <summary>
    /// Persists the in-memory chat list to browser session storage.
    /// </summary>
    public async Task SaveAsync()
    {
        // Serialise current chats into JSON before saving.
        var json = JsonSerializer.Serialize(Chats);
        await _js.InvokeVoidAsync("sessionStorage.setItem", StorageKey, json);
    }

    /// <summary>
    /// Finds a chat by session id.
    /// </summary>
    /// <param name="sessionId">Session id to search for.</param>
    /// <returns>Matching chat or <c>null</c> when not found.</returns>
    public ChatSession? GetChat(string sessionId)
    {
        return Chats.FirstOrDefault(c => c.SessionKey == sessionId);
    }

    /// <summary>
    /// Returns an existing chat or creates a new one for the provided session.
    /// </summary>
    /// <param name="sessionId">Unique chat session id.</param>
    /// <param name="sessionName">Display name for the partner/session.</param>
    /// <param name="partnerAegisId">Partner Aegis id.</param>
    /// <param name="partnerPublicKey">Partner public key used for crypto setup.</param>
    /// <returns>The existing or newly created chat session.</returns>
    public ChatSession CreateChat(string sessionId, string sessionName, string partnerAegisId, string partnerPublicKey)
    {
        // Try to reuse an existing chat first.
        var chat = Chats.FirstOrDefault(c => c.SessionKey == sessionId);

        if (chat == null)
        {
            // If there is no chat yet, create one and keep it in memory.
            chat = new ChatSession
            {
                SessionKey = sessionId,
                PartnerName = sessionName,
                PartnerAegisId = partnerAegisId,
                PartnerPublicKey = partnerPublicKey
            };

            Chats.Add(chat);
        }

        return chat;
    }
}
