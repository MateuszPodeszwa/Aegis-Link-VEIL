public class ChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public MessageSender From { get; set; } = MessageSender.System;
    public string Content { get; set; } = string.Empty;
    public bool IsEdited { get; set; } = false;
    public DateTime SendAt { get; set; }
    public DateTime DeleteAt { get; set; }

    public ChatMessage()
    {
        Id = Guid.NewGuid();
        SendAt = DateTime.UtcNow;
    }
}