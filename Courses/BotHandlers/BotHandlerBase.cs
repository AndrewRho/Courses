using Courses.Abstractions;
using Courses.Models;
using Telegram.Bot;

namespace Courses.BotHandlers;

public abstract class BotHandlerBase : IBotHandler
{
    public async Task Handle(ChatContext context)
    {
        try
        {
            await HandleSafe(context);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await context.Client.SendTextMessageAsync(context.ChatId, $"На жаль, сталася помилка. {e.Message}", cancellationToken: context.CancelToken);
        }
    }

    public virtual Type? GetNextHandlerType()
    {
        return null;
    }

    protected abstract Task HandleSafe(ChatContext context);
}