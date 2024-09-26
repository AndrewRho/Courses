using Courses.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Courses.BotHandlers;

public abstract class BotHandlerBase : IBotHandler
{
    public async Task Handle(ITelegramBotClient client, ChatId chatId, CancellationToken token)
    {
        try
        {
            await HandleSafe(client, chatId, token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await client.SendTextMessageAsync(chatId, $"На жаль, сталася помилка. {e.Message}", cancellationToken: token);
        }
    }

    public virtual IBotHandler? GetNext()
    {
        return null;
    }

    protected abstract Task HandleSafe(ITelegramBotClient client, ChatId chatId, CancellationToken token);
}