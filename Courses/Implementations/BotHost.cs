using Courses.Abstractions;
using Courses.Configs;
using Courses.Data.Entities;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Courses.Implementations;

public class BotHost : IBotHost
{
    private readonly TelegramBotClient _client;
    private readonly IBotHandlerFactory _handlerFactory;
    private readonly ICoursesBotContextFactory _contextFactory;

    public BotHost(
        IOptions<BotConfig> config,
        IBotHandlerFactory handlerFactory,
        ICoursesBotContextFactory contextFactory)
    {
        _handlerFactory = handlerFactory;
        _client = new TelegramBotClient(config.Value.TelegramToken);
        _contextFactory = contextFactory;
    }

    public void Start()
    {
        _client.StartReceiving(UpdateHandler, ErrorHandler);
    }

    private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
    {
        var handler = _handlerFactory.GetHandler(update);
        var chatContext = _handlerFactory.GetChatContext(client, update, token);
        var dbContext = _contextFactory.GetContext();
        if (!dbContext.Users.Any(x => x.Id == chatContext.UserId))
        {
            dbContext.Users.Add(new UserEntity {Id = chatContext.UserId, UserName = update.Message?.From?.Username ?? string.Empty});
            await dbContext.SaveChangesAsync(token);
        }
        
        // handlers chain. Each handler may return next non-null handler which will be processed in scope of this update
        while (handler != null)
        {
            await handler.Handle(chatContext);
            var nextHandlerType = handler.GetNextHandlerType();
            if (nextHandlerType != null)
            {
                handler = _handlerFactory.GetHandler(nextHandlerType);
            }
            else
            {
                break;
            }
        }
    }

    private static Task ErrorHandler(ITelegramBotClient client, Exception ex, CancellationToken token )
    {
        Console.WriteLine(ex.ToString());
        return Task.CompletedTask;
    }
}