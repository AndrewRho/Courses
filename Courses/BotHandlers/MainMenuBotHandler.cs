using Courses.Abstractions;
using Courses.Configs;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Courses.BotHandlers;

public class MainMenuBotHandler : IBotHandler
{
    public async Task Handle(ITelegramBotClient client, ChatId chatId, CancellationToken token)
    {
        InlineKeyboardMarkup scheduleButtons = new( 
            new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Cьогодні", "get.schedule.today"),
                    InlineKeyboardButton.WithCallbackData(text: "На тиждень", "get.schedule.week")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "До кінця семестру", "get.schedule.semester.left"),
                    InlineKeyboardButton.WithCallbackData(text: "Інформація", ActionNames.GetAllInfo),
                }
            }
        );
        
        await client.SendTextMessageAsync(chatId, "Розклад занять", replyMarkup: scheduleButtons, cancellationToken: token);
    }

    public IBotHandler? GetNext()
    {
        return null;
    }
}