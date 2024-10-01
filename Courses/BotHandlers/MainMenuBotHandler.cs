using Courses.Configs;
using Courses.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Courses.BotHandlers;

public class MainMenuBotHandler : BotHandlerBase
{
    protected override async Task HandleSafe(ChatContext context)
    {
        InlineKeyboardMarkup scheduleButtons = new( 
            new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Cьогодні", ActionNames.GetScheduleToday),
                    InlineKeyboardButton.WithCallbackData(text: "На тиждень", "get.schedule.week")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "До кінця семестру", "get.schedule.semester.left"),
                    InlineKeyboardButton.WithCallbackData(text: "Інформація", ActionNames.GetAllInfo),
                }
            }
        );
        
        await context.Client.SendTextMessageAsync(
            context.ChatId, 
            "Розклад занять",
            replyMarkup: scheduleButtons, 
            cancellationToken: context.CancelToken);
    }
}