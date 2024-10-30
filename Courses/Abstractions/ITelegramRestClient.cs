namespace Courses.Abstractions;

public interface ITelegramRestClient
{
    Task SendMessage(long chatId, string text, bool isHtml);
    Task SendStartMenuButtons(long chatId);
    Task<string[]> GetDownloadedLines(string fileId);
}