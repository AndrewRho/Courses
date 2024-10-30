namespace Courses.Models;

public class ChatContext
{
    public long ChatId { get; }
    public long UserId { get; }
    public string FileId { get; set; } = string.Empty;

    public ChatContext(long chatId, long userId)
    {
        ChatId = chatId;
        UserId = userId;
    }
}