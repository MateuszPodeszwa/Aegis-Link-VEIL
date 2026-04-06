public class ChatSession
{
    public string PartnerName { get; set; } = string.Empty;
    public string PartnerAegisId { get; set; } = string.Empty;
    public string PartnerPublicKey { get; set; } = string.Empty;
    public string SessionKey { get; set; } = string.Empty;
    public List<ChatMessage> Messages { get; set; } = new();

    public ChatMessage StoreMessage(string content, MessageSender from)
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