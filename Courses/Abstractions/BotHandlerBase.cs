using Courses.Models;

namespace Courses.Abstractions;

public abstract class BotHandlerBase : IBotHandler
{
    private readonly ITelegramRestClient _client;

    protected BotHandlerBase(ITelegramRestClient client)
    {
        _client = client;
    }

    public async Task Handle(ChatContext context)
    {
        try
        {
            await HandleSafe(context);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await Say(context, $"На жаль, сталася помилка. {e.Message}", false);
        }
    }

    public async Task Say(ChatContext context, string what, bool isHtml)
    {
        await _client.SendMessage(context.ChatId, what, isHtml);
    }

    public virtual Type? GetNextHandlerType()
    {
        return null;
    }

    protected abstract Task HandleSafe(ChatContext context);
}