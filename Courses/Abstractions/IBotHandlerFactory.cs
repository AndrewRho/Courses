using Courses.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Courses.Abstractions;

public interface IBotHandlerFactory
{
    IBotHandler? GetHandler(Update update);
    ChatContext GetChatContext(ITelegramBotClient client, Update update, CancellationToken token);
    IBotHandler? GetHandler(Type handlerType);
}