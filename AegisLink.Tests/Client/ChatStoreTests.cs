using System.Text.Json;
using AegisLink.Tests.TestUtilities;

namespace AegisLink.Tests.Client;

public class ChatStoreTests
{
    [Fact]
    public async Task InitializeAsync_LoadsStoredChats()
    {
        var storedChats = new List<global::ChatSession>
        {
            new global::ChatSession { SessionKey = "session-1", PartnerName = "Alice" }
        };
        var json = JsonSerializer.Serialize(storedChats);
        var js = new TestJsRuntime((identifier, _) =>
            identifier == "sessionStorage.getItem" ? json : null);
        var store = new global::ChatStore(js);

        await store.InitializeAsync();

        Assert.Single(store.Chats);
        Assert.Equal("session-1", store.Chats[0].SessionKey);
    }

    [Fact]
    public async Task InitializeAsync_CachesLoadTask()
    {
        var callCount = 0;
        var js = new TestJsRuntime((identifier, _) =>
        {
            if (identifier == "sessionStorage.getItem")
            {
                callCount++;
                return string.Empty;
            }

            return null;
        });
        var store = new ChatStore(js);

        await store.InitializeAsync();
        await store.InitializeAsync();

        Assert.Equal(1, callCount);
    }

    [Fact]
    public async Task SaveAsync_PersistsChats()
    {
        object?[]? savedArgs = null;
        var js = new TestJsRuntime((identifier, args) =>
        {
            if (identifier == "sessionStorage.setItem")
            {
                savedArgs = args;
            }

            return null;
        });
        var store = new global::ChatStore(js);
        store.Chats.Add(new global::ChatSession { SessionKey = "session-1" });

        await store.SaveAsync();

        Assert.NotNull(savedArgs);
        Assert.Equal("aegis_chats", savedArgs![0]);
        var savedChats = JsonSerializer.Deserialize<List<global::ChatSession>>(savedArgs![1]?.ToString() ?? string.Empty);
        Assert.NotNull(savedChats);
        Assert.Single(savedChats!);
        Assert.Equal("session-1", savedChats![0].SessionKey);
    }

    [Fact]
    public void GetChat_ReturnsChatWhenPresent()
    {
        var store = new global::ChatStore(new TestJsRuntime((_, _) => null));
        store.Chats.Add(new global::ChatSession { SessionKey = "session-1" });

        var chat = store.GetChat("session-1");

        Assert.NotNull(chat);
        Assert.Equal("session-1", chat!.SessionKey);
    }

    [Fact]
    public void CreateChat_ReusesExistingChat()
    {
        var store = new global::ChatStore(new TestJsRuntime((_, _) => null));
        var existing = store.CreateChat("session-1", "Alice", "ABCDEFGH", "pub");

        var second = store.CreateChat("session-1", "Bob", "IJKLMNO", "pub2");

        Assert.Same(existing, second);
        Assert.Equal("Alice", second.PartnerName);
    }
}
