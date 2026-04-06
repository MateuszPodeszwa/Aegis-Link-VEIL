using Microsoft.JSInterop;
using System.Text.Json;

public class ChatStore
{
    private readonly IJSRuntime _js;
    private const string StorageKey = "aegis_chats";
    private bool _initialized = false;

    public List<ChatSession> Chats { get; private set; } = new();

    public ChatStore(IJSRuntime js)
    {
        _js = js;
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        _initialized = true;

        var json = await _js.InvokeAsync<string>("sessionStorage.getItem", StorageKey);

        if (!string.IsNullOrEmpty(json))
        {
            Chats = JsonSerializer.Deserialize<List<ChatSession>>(json)
                    ?? new List<ChatSession>();
        }
    }

    public async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(Chats);
        await _js.InvokeVoidAsync("sessionStorage.setItem", StorageKey, json);
    }

    public ChatSession? GetChat(string sessionId)
    {
        return Chats.FirstOrDefault(c => c.SessionKey == sessionId);
    }

    public ChatSession CreateChat(string sessionId, string sessionName)
    {
        var chat = Chats.FirstOrDefault(c => c.SessionKey == sessionId);

        if (chat == null)
        {
            chat = new ChatSession
            {
                SessionKey = sessionId,
                PartnerName = sessionName
            };

            Chats.Add(chat);
        }

        return chat;
    }
}