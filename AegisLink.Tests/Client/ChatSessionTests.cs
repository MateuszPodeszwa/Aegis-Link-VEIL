namespace AegisLink.Tests.Client;

public class ChatSessionTests
{
    [Fact]
    public void StoreMessage_AppendsMessage()
    {
        var session = new global::ChatSession();
        var message = new global::ChatMessage { Content = "hello" };

        session.StoreMessage(message);

        Assert.Single(session.Messages);
        Assert.Equal(message, session.Messages[0]);
    }

    [Fact]
    public void DeleteMessage_RemovesMatchingMessage()
    {
        var session = new global::ChatSession();
        var message = new global::ChatMessage { Content = "hello" };
        session.StoreMessage(message);

        session.DeleteMessage(message.Id);

        Assert.Empty(session.Messages);
    }

    [Fact]
    public void DeleteMessage_IgnoresMissingMessage()
    {
        var session = new global::ChatSession();
        session.StoreMessage(new global::ChatMessage());

        session.DeleteMessage(Guid.NewGuid());

        Assert.Single(session.Messages);
    }
}
