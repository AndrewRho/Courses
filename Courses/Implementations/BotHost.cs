using Courses.Abstractions;
using Courses.Configs;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Courses.Implementations;

public class BotHost : IBotHost
{
    private readonly TelegramBotClient _client;
    private readonly IBotHandlerFactory _factory;

    public BotHost(IOptions<BotConfig> config, IBotHandlerFactory factory)
    {
        _factory = factory;
        _client = new TelegramBotClient(config.Value.TelegramToken);
    }

    public void Start()
    {
        _client.StartReceiving(UpdateHandler, ErrorHandler);
    }

    private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
    {
        var handler = _factory.GetHandler(update);
        var chatId = _factory.GetChatId(update);
        
        // handlers chain. Each handler may return next non-null handler which will be processed in scope of this update
        while (handler != null)
        {
            await handler.Handle(client, chatId, token);
            handler = handler.GetNext();
        }
    }

    private static Task ErrorHandler(ITelegramBotClient client, Exception ex, CancellationToken token )
    {
        Console.WriteLine(ex.ToString());
        return Task.CompletedTask;
    }
}