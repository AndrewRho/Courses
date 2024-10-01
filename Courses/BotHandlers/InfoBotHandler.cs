using Courses.Abstractions;
using Courses.Models;

namespace Courses.BotHandlers;

public class InfoBotHandler : BotHandlerBase
{
    private readonly ITableRenderService _render; 

    public InfoBotHandler(ITableRenderService render)
    {
        _render = render;
    }

    public override Type GetNextHandlerType()
    {
        return typeof(MainMenuBotHandler);
    }
    
    protected override async Task HandleSafe(ChatContext context)
    {
        /*
        var models = _disciplineRepository.GetAllWithTopics();
        foreach (var m in models)
        {
            var table = _render.GetAllDisciplineInfo(m);
            await client.SendTextMessageAsync(chatId, table, parseMode: ParseMode.Html, cancellationToken: token);
        }
        */
    }
}