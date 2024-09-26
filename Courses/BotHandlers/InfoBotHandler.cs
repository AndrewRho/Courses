using Courses.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Courses.BotHandlers;

public class InfoBotHandler : BotHandlerBase
{
    private readonly IDisciplineRepository _disciplineRepository;
    private readonly ITableRenderService _render; 

    public InfoBotHandler(IDisciplineRepository disciplineRepository, ITableRenderService render)
    {
        _disciplineRepository = disciplineRepository;
        _render = render;
    }

    public override IBotHandler GetNext()
    {
        return new MainMenuBotHandler();
    }
    
    protected override async Task HandleSafe(ITelegramBotClient client, ChatId chatId, CancellationToken token)
    {
        var models = _disciplineRepository.GetAllWithTopics();
        foreach (var m in models)
        {
            var table = _render.GetAllDisciplineInfo(m);
            await client.SendTextMessageAsync(chatId, table, parseMode: ParseMode.Html, cancellationToken: token);
        }
    }
}