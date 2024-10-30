namespace Courses.Configs;

public class BotConfig
{
    public string TelegramToken { get; set; } = string.Empty;
    public string ConnString { get; set; } = string.Empty;
    public string TelegramDownloadUrl { get; set; } = string.Empty;
    public long AdminUserId { get; set; }
    
    public bool IsDebugOutput { get; set; }
}