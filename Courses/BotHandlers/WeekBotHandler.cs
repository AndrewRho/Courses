using Courses.Abstractions;

namespace Courses.BotHandlers;

public class WeekBotHandler : ScheduleBotHandlerBase
{
    public WeekBotHandler(
        ITelegramRestClient restClient,
        ITableRenderService tableRender,
        IScheduleService scheduleService) : base(restClient, tableRender, scheduleService)
    {
    }

    protected override (DateTime dateFrom, DateTime dateTo) GetTimeLimits()
    {
        var now = DateTime.UtcNow.Date;
        var nextWeek = now.AddDays(7);
        return new (now, nextWeek);
    }
}