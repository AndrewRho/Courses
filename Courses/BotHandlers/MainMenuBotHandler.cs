using Courses.Abstractions;
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
                    InlineKeyboardButton.WithCallbackData(text: "На тиждень", ActionNames.GetScheduleWeek)
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "До кінця семестру", ActionNames.GetScheduleSemesterLeft),
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