using Courses.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Courses.BotHandlers;

public class InfoBotHandler : IBotHandler
{
    private readonly IDisciplineRepository _disciplineRepository;
    private readonly ITableRenderService _render; 

    public InfoBotHandler(IDisciplineRepository disciplineRepository, ITableRenderService render)
    {
        _disciplineRepository = disciplineRepository;
        _render = render;
    }

    public async Task Handle(ITelegramBotClient client, ChatId chatId, CancellationToken token)
    {
        var models = _disciplineRepository.GetAllWithTopics();
        var table = _render.GetAllDisciplinesInfo(models);
        await client.SendTextMessageAsync(chatId, table, parseMode: ParseMode.Html, cancellationToken: token);
    }

    public IBotHandler? GetNext()
    {
        return new MainMenuBotHandler();
    }
}