using System.Text.RegularExpressions;
using Courses.Abstractions;
using Courses.Configs;
using Courses.Extensions;
using Courses.Models;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Courses.BotHandlers;

public class DownloadTextFileBotHandler : BotHandlerBase
{
    private static HttpClient? _client;
    private readonly BotConfig _config;
    private readonly string _fileId;
    private readonly IDisciplineRepository _disciplineRepository;

    public DownloadTextFileBotHandler(
        BotConfig config,
        string fileId,
        IDisciplineRepository disciplineRepository)
    {
        _config = config;
        _fileId = fileId;
        _disciplineRepository = disciplineRepository;
    }
    
    public override IBotHandler GetNext()
    {
        return new MainMenuBotHandler();
    }

    protected override async Task HandleSafe(ITelegramBotClient client, ChatId chatId, CancellationToken token)
    {
        var httpClient = GetHttpClient();
        var request = $"/bot{_config.TelegramToken}/getFile?file_id={_fileId}";
        var contents = await httpClient.GetStringAsync(request, token);
        var parsed = JsonConvert.DeserializeObject < GetFileResponse>(contents);
        
        request = $"/file/bot{_config.TelegramToken}/{parsed?.Result.file_path}";
        contents = await httpClient.GetStringAsync(request, token);
        var lines = Regex.Split(contents, "\r\n").Select( x => x.Trim().Prettify()).ToArray();
        var firstLine = lines.First(x => !string.IsNullOrWhiteSpace(x));

        if (firstLine.Equals("Розклад занять"))
        {
            await ProcessSchedule(client, chatId, lines, token);
        }
        else
        {
            await ProcessDiscipline(client, chatId, firstLine, lines, token);
        }
    }
    
    private async Task ProcessSchedule(ITelegramBotClient client, ChatId chatId, string[] lines, CancellationToken token)
    {
        await client.SendTextMessageAsync(chatId, "Отримано розклад занять", cancellationToken: token);
        var slots = _disciplineRepository.GetTimeSlots();
        var curDate = string.Empty;

        var models = new List<ScheduleModel>();
        
        for (var i = 0; i < lines.Length - 3; i++)
        {
            var signature = lines[i] + "/" + lines[i + 1];
            var slot = slots.FirstOrDefault(x => signature.StartsWith(x.GetSignature()));
            if (slot == null)
            {
                continue;
            }

            // first pair, before it date comes
            if (slot.Id == 1)
            {
                curDate = lines[i - 1].Split(' ').First();
            }

            if (!lines[i+1].Contains("\t"))
            {
                continue;
            }

            var disciplineName = lines[i + 3];
            var isLecture = disciplineName.EndsWith("(Л)");
            var words = disciplineName.Split(' ');
            disciplineName = string.Join(" ", words.SkipLast(1));
            
            models.Add(new ScheduleModel
            {
                TimeSlotId = slot.Id,
                DisciplineName = disciplineName,
                IsLecture = isLecture,
                SlotDate = DateTime.Parse(curDate)
            });
        }

        var uniqueNames = models.Select(x => x.DisciplineName).Distinct().ToArray();

        foreach (var u in uniqueNames)
        {
            var disciplineId = _disciplineRepository.GetId(u); 
            if (disciplineId < 0 )
            {
                throw new Exception($"Дисціплина {u} не співпадає з назвою в базі даних");
            }

            foreach (var m in models.Where( x => x.DisciplineName == u))
            {
                m.DisciplineId = disciplineId;
            }
        }

        _disciplineRepository.UpdateSchedule(models.ToArray());

    }

    private async Task ProcessDiscipline(ITelegramBotClient client, ChatId chatId, string firstLine, string[] lines, CancellationToken token)
    {
        await client.SendTextMessageAsync(chatId, "Отримано план викладання дисціплини " + firstLine, cancellationToken: token);
        var disciplines = _disciplineRepository.GetList();
        var existing = disciplines.FirstOrDefault(x => x.DisciplineName.Equals(firstLine, StringComparison.InvariantCultureIgnoreCase));

        if (existing == null)
        {
            await client.SendTextMessageAsync(chatId, "Це - нова дисціплина, створюю записи", cancellationToken: token);
            var id = _disciplineRepository.Create(firstLine);
            existing = new DisciplineModel
            {
                DisciplineId = id,
                DisciplineName = firstLine
            };
        }
        var topics = GetTopics(lines, existing.DisciplineId);
        _disciplineRepository.UpdateTopics(topics);
        
    }

    private static TopicModel[] GetTopics(string[] lines, int disciplineId)
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