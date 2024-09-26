using Telegram.Bot.Types;

namespace Courses.Abstractions;

public interface IBotHandlerFactory
{
    IBotHandler? GetHandler(Update update);
    ChatId GetChatId(Update update);
}