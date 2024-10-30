using Courses.Abstractions;
using Courses.BotHandlers;
using Courses.Configs;
using Courses.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Courses.Factories;

public class BotHandlerFactory : IBotHandlerFactory
{
    private readonly IServiceProvider _provider;
    
    public BotHandlerFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IBotHandler? GetHandler(Update update)
    {
        if (update.Message?.Type == MessageType.Document)
        {
            return _provider.GetRequiredService<TextFileBotHandler>();
        }
        
        var message = ( update.Message?.Text ?? string.Empty ).ToLowerInvariant();
        var chatId = update.Message?.Chat.Id ?? -1;

        if (!string.IsNullOrEmpty(message) && chatId >= 0)
        {
            return GetActionHandler(message);
        }
        
        if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery?.Message != null)
        {
            var callback = (update.CallbackQuery?.Data ?? string.Empty).ToLowerInvariant();
            chatId = update.CallbackQuery?.Message.Chat.Id ?? -1;
            if (!string.IsNullOrEmpty(callback) && chatId >= 0)
            {
                return GetActionHandler(callback);
            }
        }

        return null;
    }

    public IBotHandler? GetHandler(Type handlerType)
    {
        return _provider.GetRequiredService(handlerType) as IBotHandler;
    }

    private IBotHandler? GetActionHandler(string actionMessage)
    {
        switch (actionMessage)
        {
            case ActionNames.Start : return _provider.GetRequiredService<StartBotHandler>();
            case ActionNames.ViewMainMenu: return _provider.GetRequiredService<MainMenuBotHandler>();
            case ActionNames.GetAllInfo : return _provider.GetRequiredService<InfoBotHandler>();
            case ActionNames.GetScheduleToday : return _provider.GetRequiredService<TodayBotHandler>();
            case ActionNames.GetScheduleWeek : return _provider.GetRequiredService<WeekBotHandler>();
            case ActionNames.GetScheduleSemesterLeft : return _provider.GetRequiredService<SemesterBotHandler>();
        }

        return null;
    }
}