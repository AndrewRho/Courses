using Telegram.Bot;
using Telegram.Bot.Types;

namespace Courses.Abstractions;

public interface IBotHandler
{
    Task Handle(ITelegramBotClient client, ChatId chatId, CancellationToken token);

    IBotHandler? GetNext();
}