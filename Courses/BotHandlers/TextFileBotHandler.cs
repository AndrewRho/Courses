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
    private readonly ITelegramRestClient _restClient;
    private readonly BotConfig _config;

    public TextFileBotHandler(
        IOptions<BotConfig> config,
        ITelegramRestClient restClient,
        IWorkPlanService workPlanService,
        IScheduleService scheduleService) : base(restClient)
    {
        _config = config.Value;
        _scheduleService = scheduleService;
        _workPlanService = workPlanService;
        _restClient = restClient;
    }
    
    public override Type GetNextHandlerType()
    {
        return typeof(MainMenuBotHandler);
    }

    protected override async Task HandleSafe(ChatContext context)
    {
        var lines = await _restClient.GetDownloadedLines(context.FileId);
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
}