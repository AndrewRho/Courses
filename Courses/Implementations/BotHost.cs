using Courses.Abstractions;
using Courses.Configs;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Courses.Implementations;

public class BotHost : IBotHost
{
    private readonly TelegramBotClient _client;

    public BotHost(IOptions<BotConfig> config)
    {
        _client = new TelegramBotClient(config.Value.TelegramToken);
    }

    public void Start()
    {
        _client.StartReceiving(UpdateHandler, ErrorHandler);
    }

    private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
    {
        if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery?.Message != null)
        {
            await client.SendTextMessageAsync(
                update.CallbackQuery.Message.Chat.Id,
                "Got it",
                cancellationToken: token);
            return;
        }
        
        InlineKeyboardMarkup inlineKeyboard = new( new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Button 1", callbackData: "callback 1"),
                InlineKeyboardButton.WithCallbackData(text: "Button 2", callbackData: "callback 2"),
            },
 
        });


        if (update.Message != null)
        {
            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                "Buttons",
                replyMarkup: inlineKeyboard,
                cancellationToken: token);
        }

        throw new ArgumentException("Unexpected chat ID");
    }

    private static Task ErrorHandler(ITelegramBotClient client, Exception ex, CancellationToken token )
    {
        Console.WriteLine(ex.ToString());
        return Task.CompletedTask;
    }
}