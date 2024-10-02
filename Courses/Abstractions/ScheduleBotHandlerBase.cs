using Courses.BotHandlers;
using Courses.Models;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Courses.Abstractions;

public abstract class ScheduleBotHandlerBase : BotHandlerBase
{
    private readonly ITableRenderService _tableRender;
    private readonly IScheduleService _scheduleService;

    protected ScheduleBotHandlerBase(
        ITableRenderService tableRender,
        IScheduleService scheduleService)
    {
        _tableRender = tableRender;
        _scheduleService = scheduleService;
    }
    public override Type GetNextHandlerType()
    {
        return typeof(MainMenuBotHandler);
    }

    protected abstract (DateTime dateFrom, DateTime dateTo) GetTimeLimits();
    
    protected override async Task HandleSafe(ChatContext context)
    {
        var limits = GetTimeLimits();
        var schedules = _scheduleService.GetSchedule(limits.dateFrom, limits.dateTo, context.UserId);
        var grouped = schedules.GroupBy(x => x.Date);

        foreach (var g in grouped)
        {
            await context.Say(g.Key.ToLongDateString());
            var table = _tableRender.GetScheduleInfo(g.ToArray());
            await context.Client.SendTextMessageAsync(context.ChatId, table, parseMode: ParseMode.Html, cancellationToken: context.CancelToken);
        }
    }
}