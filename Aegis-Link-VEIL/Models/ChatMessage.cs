public class ChatMessage
{
    public MessageSender From { get; set; } = MessageSender.System;
    public string Content { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
}