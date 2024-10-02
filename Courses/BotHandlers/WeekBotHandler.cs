using Courses.Abstractions;

namespace Courses.BotHandlers;

public class WeekBotHandler : ScheduleBotHandlerBase
{
    public WeekBotHandler(
        ITableRenderService tableRender,
        IScheduleService scheduleService) : base(tableRender, scheduleService)
    {
    }

    protected override (DateTime dateFrom, DateTime dateTo) GetTimeLimits()
    {
        var now = DateTime.Now.Date;
        var nextWeek = now.AddDays(7);
        return new (now, nextWeek);
    }
}