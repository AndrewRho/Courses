using Courses.Models;

namespace Courses.BotHandlers;

public class StartBotHandler : BotHandlerBase
{
    protected override async Task HandleSafe(ChatContext context)
    {
        await context.Say( GetGreeting() + " Оберіть потрібну дію.");
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