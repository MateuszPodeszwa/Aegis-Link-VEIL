public class ChatSession
{
    public string PartnerName { get; set; } = string.Empty;
    public string PartnerAegisId { get; set; } = string.Empty;
    public string PartnerPublicKey { get; set; } = string.Empty;
    public string SessionKey { get; set; } = string.Empty;
    public List<ChatMessage> Messages { get; set; } = new();

    public void StoreMessage(ChatMessage message)
    {
        Messages.Add(message);
    }

    public void DeleteMessage(Guid messageId)
    {
        var message = Messages.FirstOrDefault(m => m.Id == messageId);

        if (message != null)
        {
            Messages.Remove(message);
        }
    }
}