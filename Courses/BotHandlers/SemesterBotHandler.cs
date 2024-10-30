using Courses.Abstractions;

namespace Courses.BotHandlers;

public class SemesterBotHandler : ScheduleBotHandlerBase
{
    public SemesterBotHandler(
        ITelegramRestClient restClient,
        ITableRenderService tableRender,
        IScheduleService scheduleService) : base(restClient, tableRender, scheduleService)
    {
    }

    protected override (DateTime dateFrom, DateTime dateTo) GetTimeLimits()
    {
        var now = DateTime.UtcNow.Date;
        var isAutumn = now.Month >= 9;

        var semesterEnds = isAutumn
            ? new DateTime(now.Year, 12, 31, 0, 0, 0,0, DateTimeKind.Utc)
            : new DateTime(now.Year, 7, 1, 0, 0, 0,0, DateTimeKind.Utc);

        return new(now, semesterEnds);
    }
}