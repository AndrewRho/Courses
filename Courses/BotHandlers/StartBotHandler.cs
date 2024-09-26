using Courses.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Courses.BotHandlers;

public class StartBotHandler : BotHandlerBase
{
    protected override async Task HandleSafe(ITelegramBotClient client, ChatId chatId, CancellationToken token)
    {
        await client.SendTextMessageAsync(chatId, GetGreeting() + " Оберіть потрібну дію.", cancellationToken: token);
    }

    public override IBotHandler GetNext()
    {
        return new MainMenuBotHandler();
    }

    private static string GetGreeting()
    {
        var hour = DateTime.Now.Hour;
        return hour switch
        {
            >= 6 and <= 11 => "Доброго ранку.",
            <= 17 => "Доброго дня.",
            <= 23 => "Доброго вечора.",
            _ => "Доброї ночі."
        };
    }
}