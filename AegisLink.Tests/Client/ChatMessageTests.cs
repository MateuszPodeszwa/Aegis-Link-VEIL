namespace AegisLink.Tests.Client;

public class ChatMessageTests
{
    [Fact]
    public void Defaults_AreInitialized()
    {
        var start = DateTime.UtcNow;
        var message = new global::ChatMessage();
        var end = DateTime.UtcNow;

        Assert.NotEqual(Guid.Empty, message.Id);
        Assert.Equal(global::MessageSender.System, message.From);
        Assert.Equal(string.Empty, message.Content);
        Assert.False(message.IsEdited);
        Assert.InRange(message.SendAt, start, end);
    }
}
