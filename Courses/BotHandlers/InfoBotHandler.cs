using Courses.Abstractions;
using Courses.Models;
using Microsoft.EntityFrameworkCore;

namespace Courses.BotHandlers;

public class InfoBotHandler : BotHandlerBase
{
    private readonly ITableRenderService _render;
    private readonly ICoursesBotContextFactory _contextFactory;

    public InfoBotHandler(
        ITelegramRestClient restClient,
        ITableRenderService render,
        ICoursesBotContextFactory contextFactory) : base(restClient)
    {
        _render = render;
        _contextFactory = contextFactory;
    }

    public override Type GetNextHandlerType()
    {
        return typeof(MainMenuBotHandler);
    }
    
    protected override async Task HandleSafe(ChatContext context)
    {
        using var dbContext = _contextFactory.GetContext();
        var entities = dbContext.Disciplines
            .Include(x => x.Topics)
            .ThenInclude(x => x.WorkPlans)
            .Where(x => x.Topics.Single().WorkPlans.Single().User.Id == context.UserId)
            .ToArray();


        var models = new List<DisciplineModel>();
        foreach (var e in entities)
        {
            var model = new DisciplineModel
            {
                DisciplineName = e.Name,
                TotalLectures = e.Topics.SelectMany(x => x.WorkPlans).Sum(x => x.Lectures),
                TotalPractices = e.Topics.SelectMany(x => x.WorkPlans).Sum(x => x.Practices),
                Topics = e.Topics.Select(t => new TopicModel
                {
                    TopicName = t.Name,
                    TopicNumber = t.Number,
                    Lectures = t.WorkPlans.Sum(x => x.Lectures),
                    Practices = t.WorkPlans.Sum(x => x.Practices)
                }).ToArray()
            };
            
            models.Add(model);
        }

        foreach (var m in models)
        {
            var table = _render.GetAllDisciplineInfo(m);
            await Say(context, table, true);
        }
    }
}