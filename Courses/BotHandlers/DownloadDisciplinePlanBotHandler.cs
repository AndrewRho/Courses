using System.Text.RegularExpressions;
using Courses.Abstractions;
using Courses.Configs;
using Courses.Extensions;
using Courses.Models;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Courses.BotHandlers;

public class DownloadDisciplinePlanBotHandler : IBotHandler
{

    private static HttpClient? _client;
    private readonly BotConfig _config;
    private readonly string _fileId;
    private readonly IDisciplineRepository _disciplineRepository;

    public DownloadDisciplinePlanBotHandler(
        BotConfig config,
        string fileId,
        IDisciplineRepository disciplineRepository)
    {
        _config = config;
        _fileId = fileId;
        _disciplineRepository = disciplineRepository;
    }
    
    public async Task Handle(ITelegramBotClient client, ChatId chatId, CancellationToken token)
    {
        try
        {
            await HandleSafe(client, chatId, token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await client.SendTextMessageAsync(chatId, $"На жаль, сталася помилка. {e.Message}", cancellationToken: token);
        }
    }

    public IBotHandler? GetNext()
    {
        return new MainMenuBotHandler();
    }

    private async Task HandleSafe(ITelegramBotClient client, ChatId chatId, CancellationToken token)
    {
        var httpClient = GetHttpClient();
        var request = $"/bot{_config.TelegramToken}/getFile?file_id={_fileId}";
        var contents = await httpClient.GetStringAsync(request, token);
        var parsed = JsonConvert.DeserializeObject < GetFileResponse>(contents);
        
        request = $"/file/bot{_config.TelegramToken}/{parsed?.Result.file_path}";
        contents = await httpClient.GetStringAsync(request, token);
        var lines = Regex.Split(contents, "\r\n").Select( x => x.Trim().Prettify()).ToArray();
        var disciplineName = lines.First(x => !string.IsNullOrWhiteSpace(x));
        
        await client.SendTextMessageAsync(chatId, "Отримано план викладання дисціплини " + disciplineName, cancellationToken: token);
        var disciplines = _disciplineRepository.GetList();
        var existing = disciplines.FirstOrDefault(x => x.DisciplineName.Equals(disciplineName, StringComparison.InvariantCultureIgnoreCase));

        if (existing == null)
        {
            await client.SendTextMessageAsync(chatId, "Це - нова дисціплина, створюю записи", cancellationToken: token);
            var id = _disciplineRepository.Create(disciplineName);
            existing = new DisciplineModel
            {
                DisciplineId = id,
                DisciplineName = disciplineName
            };
        }
        var topics = GetTopics(lines, existing.DisciplineId);
        _disciplineRepository.UpdateTopics(topics);
    }

    private TopicModel[] GetTopics(string[] lines, int disciplineId)
    {
        var topics = lines
            .Where(x => x.StartsWith("тема", StringComparison.InvariantCultureIgnoreCase))
            .ToArray();

        var models = new List<TopicModel>();

        var ix = 0;
        foreach (var t in topics)
        {
            var tabSplit = t
                .Split('\t')
                .Where( x => !string.IsNullOrWhiteSpace(x))
                .Take(4)
                .ToArray();
            
            ix++;

            var topic = new TopicModel
            {
                DisciplineId = disciplineId,
                TopicName = tabSplit.First(),
                TopicNumber = ix,
                Lectures = GetLowered(tabSplit[2]),
                Practices = GetLowered(tabSplit[3])
            };
            
            models.Add(topic);
        }

        return models.ToArray();
    }

    private static int GetLowered(string code)
    {
        // either 6 or 6/4* meaning number of hours decreased due to low number of students
        if (code.Contains('/'))
        {
            return int.Parse(code.Split('/')[1].Split('*')[0]);
        }

        return int.Parse(code);
    }
    
    private HttpClient GetHttpClient()
    {
        lock ("telegram.download")
        {
            _client ??= new HttpClient
            {
                BaseAddress = new Uri(_config.TelegramDownloadUrl)
            };
        }

        return _client;
    }
}