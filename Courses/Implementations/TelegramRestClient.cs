using System.Text;
using System.Text.RegularExpressions;
using Courses.Abstractions;
using Courses.Configs;
using Courses.Extensions;
using Courses.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Courses.Implementations;

public class TelegramRestClient : ITelegramRestClient
{
    private readonly BotConfig _config;
    private HttpClient? _downloadClient;

    public TelegramRestClient(IOptions<BotConfig> config)
    {
        _config = config.Value;
    }

    public async Task SendMessage(long chatId, string text, bool isHtml)
    {
        var token = _config.TelegramToken;
        var parseMode = isHtml ? "&parse_mode=HTML" : string.Empty;
        var url = $"https://api.telegram.org/bot{token}/sendMessage?chat_id={chatId}{parseMode}&text={text}";

        using var httpClient = new HttpClient();
        await httpClient.GetAsync(url);
    }
    
    public async Task SendStartMenuButtons(long chatId)
    {
        var token = _config.TelegramToken; 
        
        var replyMarkup = new
        {
            inline_keyboard = new[]
            {
                new[]
                {
                    new { text = "Сьогодні", callback_data = ActionNames.GetScheduleToday},
                    new { text = "На тиждень", callback_data = ActionNames.GetScheduleWeek }
                },
                new[]
                {
                    new { text = "До кінця семестру", callback_data = ActionNames.GetScheduleSemesterLeft },
                    new { text = "Інформація", callback_data = ActionNames.GetAllInfo }
                }
            }
        };
        
        var messageData = new
        {
            chat_id = chatId,
            text = "Розклад занять:",
            reply_markup = replyMarkup
        };

        var json = JsonConvert.SerializeObject(messageData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var httpClient = new HttpClient();
        await httpClient.PostAsync($"https://api.telegram.org/bot{token}/sendMessage", content);
    }

    public async Task<string[]> GetDownloadedLines(string fileId)
    {
        var httpClient = GetHttpClient();
        var request = $"/bot{_config.TelegramToken}/getFile?file_id={fileId}";
        var contents = await httpClient.GetStringAsync(request);
        var parsed = JsonConvert.DeserializeObject<GetFileResponse>(contents);
        
        request = $"/file/bot{_config.TelegramToken}/{parsed?.Result.FilePath}";
        contents = await httpClient.GetStringAsync(request);
        var lines = Regex.Split(contents, "\r\n").Select( x => x.Trim().Prettify()).ToArray();
        return lines;
    }
    
    private HttpClient GetHttpClient()
    {
        lock ("telegram.download")
        {
            _downloadClient ??= new HttpClient
            {
                BaseAddress = new Uri(_config.TelegramDownloadUrl)
            };
        }

        return _downloadClient;
    }

}