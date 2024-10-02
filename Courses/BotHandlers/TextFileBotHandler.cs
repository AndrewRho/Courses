using System.Text.RegularExpressions;
using Courses.Abstractions;
using Courses.Configs;
using Courses.Extensions;
using Courses.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Courses.BotHandlers;

public class TextFileBotHandler : BotHandlerBase
{
    private static HttpClient? _client;

    private readonly IScheduleService _scheduleService;
    private readonly IWorkPlanService _workPlanService;
    private readonly BotConfig _config;

    public TextFileBotHandler(
        IOptions<BotConfig> config,
        IWorkPlanService workPlanService,
        IScheduleService scheduleService)
    {
        _config = config.Value;
        _scheduleService = scheduleService;
        _workPlanService = workPlanService;
    }
    
    public override Type GetNextHandlerType()
    {
        return typeof(MainMenuBotHandler);
    }

    protected override async Task HandleSafe(ChatContext context)
    {
        var httpClient = GetHttpClient();
        var request = $"/bot{_config.TelegramToken}/getFile?file_id={context.FileId}";
        var contents = await httpClient.GetStringAsync(request, context.CancelToken);
        var parsed = JsonConvert.DeserializeObject < GetFileResponse>(contents);
        
        request = $"/file/bot{_config.TelegramToken}/{parsed?.Result.FilePath}";
        contents = await httpClient.GetStringAsync(request, context.CancelToken);
        var lines = Regex.Split(contents, "\r\n").Select( x => x.Trim().Prettify()).ToArray();
        var firstLine = lines.First(x => !string.IsNullOrWhiteSpace(x));

        if (firstLine.Equals("Розклад занять"))
        {
            await _scheduleService.ProcessTextFile(context, lines);
        }
        else
        {
            await _workPlanService.Process(context, lines);
        }
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