using Courses.Abstractions;

namespace Courses.BotHandlers;

public class TodayBotHandler : ScheduleBotHandlerBase
{
    public TodayBotHandler(
        ITelegramRestClient restClient,
        ITableRenderService tableRender,
        IScheduleService scheduleService) : base(restClient, tableRender, scheduleService)
    {
    }

    protected override (DateTime dateFrom, DateTime dateTo) GetTimeLimits()
    {
        var now = DateTime.UtcNow.Date;
        var nextDay = now.AddDays(1);

        return new (now, nextDay);
    }
}