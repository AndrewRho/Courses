using Courses.Abstractions;
using Courses.Models;
using Microsoft.EntityFrameworkCore;

namespace Courses.BotHandlers;

public class TodayBotHandler : BotHandlerBase
{
    private readonly ICoursesBotContextFactory _contextFactory;

    public TodayBotHandler(ICoursesBotContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    protected override async Task HandleSafe(ChatContext context)
    {
        var todayStarts = DateTime.Now.Date;
        var todayEnds = todayStarts.AddDays(1);

        using var dbContext = _contextFactory.GetContext();
        var schedules = dbContext.Schedules
            .Include( x=> x.TimeSlot)
            .Include( x => x.WorkPlan)
            .ThenInclude( x => x.Topic)
            .ThenInclude( x => x.Discipline)
            .Where(x => x.WorkPlan.User.Id == context.UserId)
            .Where(x => x.Date >= todayStarts && x.Date <= todayEnds)
            .ToArray();

        foreach (var s in schedules)
        {
           await context.Say(s.TimeSlot.TimeFrom + ", " + s.WorkPlan.Topic.Discipline.Name + ", " + s.WorkPlan.Topic.Name);
        }
    }

    public override Type GetNextHandlerType()
    {
        return typeof(MainMenuBotHandler);
    }
}