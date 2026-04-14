public class ChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public MessageSender From { get; set; } = MessageSender.System;
    public string Content { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
}