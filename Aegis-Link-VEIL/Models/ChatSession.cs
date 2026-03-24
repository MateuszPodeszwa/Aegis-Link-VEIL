public class ChatSession
{
    public string PartnerName { get; set; } = string.Empty;
    public string SessionKey { get; set; } = string.Empty;
    public List<ChatMessage> Messages { get; set; } = new();

    public ChatMessage StoreMessage(string content, string from)
    {
        ChatMessage message = new ChatMessage
        {
            Content = content,
            From = from
        };

        Messages.Add(message);
        return message;
    }
}