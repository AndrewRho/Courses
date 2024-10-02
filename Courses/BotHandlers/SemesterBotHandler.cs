using Courses.Abstractions;

namespace Courses.BotHandlers;

public class SemesterBotHandler : ScheduleBotHandlerBase
{
    public SemesterBotHandler(
        ITableRenderService tableRender,
        IScheduleService scheduleService) : base(tableRender, scheduleService)
    {
    }

    protected override (DateTime dateFrom, DateTime dateTo) GetTimeLimits()
    {
        var now = DateTime.Now.Date;
        var isAutumn = now.Month >= 9;

        var semesterEnds = isAutumn
            ? new DateTime(now.Year, 12, 31)
            : new DateTime(now.Year, 7, 1);

        return new(now, semesterEnds);
    }
}