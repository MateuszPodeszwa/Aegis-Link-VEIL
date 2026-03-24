public class ChatMessage
{
    public string From {  get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
}