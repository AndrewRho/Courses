using Courses.Abstractions;
using Courses.Models;

namespace Courses.BotHandlers;

public class StartBotHandler : BotHandlerBase
{
    public StartBotHandler(ITelegramRestClient client) : base(client)
    {
    }

    protected override async Task HandleSafe(ChatContext context)
    {
        await Say( context, GetGreeting() + " Оберіть потрібну дію.", false);
    }

    public override Type GetNextHandlerType()
    {
        return typeof(MainMenuBotHandler);
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