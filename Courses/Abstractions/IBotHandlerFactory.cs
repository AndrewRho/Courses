using Telegram.Bot.Types;

namespace Courses.Abstractions;

public interface IBotHandlerFactory
{
    IBotHandler? GetHandler(Update update);
    IBotHandler? GetHandler(Type handlerType);
}