using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Courses;

public class BotHost
{
    private readonly string _token;
    private readonly TelegramBotClient _client;

    public BotHost(string token)
    {
        _token = token;
        _client = new TelegramBotClient(token);
    }

    public void Start()
    {
        _client.StartReceiving(UpdateHandler, ErrorHandler);
        
    }

    private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
    {
        if (update.Type == UpdateType.CallbackQuery)
        {
            await client.SendTextMessageAsync(
                update.CallbackQuery.Message.Chat.Id,
                "Got it",
                cancellationToken: token);
            return;
        }
        
        
        InlineKeyboardMarkup inlineKeyboard = new( new[]
        {
            // first row
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Button 1", callbackData: "callback 1"),
                InlineKeyboardButton.WithCallbackData(text: "Button 2", callbackData: "callback 2"),
            },
 
        });
        
        
        await client.SendTextMessageAsync(
            update.Message.Chat.Id,
            "Buttons",
            replyMarkup: inlineKeyboard,
            cancellationToken: token);
    }

    private async Task ErrorHandler(ITelegramBotClient client, Exception ex, CancellationToken token )
    {
        var q = 0;
    }
}