using Courses.Abstractions;
using Courses.Configs;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Courses.BotHandlers;

public class BotHandlerFactory : IBotHandlerFactory
{
    private readonly IDisciplineRepository _disciplineRepository;
    private readonly ITableRenderService _render;
    private readonly BotConfig _config;

    public BotHandlerFactory(
        IOptions<BotConfig> config,
        IDisciplineRepository disciplineRepository, 
        ITableRenderService render)
    {
        _disciplineRepository = disciplineRepository;
        _render = render;
        _config = config.Value;
    }

    public IBotHandler? GetHandler(Update update)
    {
        if (update.Message?.Type == MessageType.Document)
        {
            // we expect work plans or schedules in .TXT format, UTF-8
            return new DownloadTextFileBotHandler(
                _config,
                update.Message?.Document?.FileId ?? string.Empty,
                _disciplineRepository);
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

    public ChatId GetChatId(Update update)
    {
        var chatId = update.Message?.Chat.Id ?? -1;
        if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery?.Message != null)
        {
            chatId = update.CallbackQuery?.Message.Chat.Id ?? -1;
        }

        return chatId;
    }

    private IBotHandler? GetActionHandler(string actionMessage)
    {
        switch (actionMessage)
        {
            case ActionNames.Start : return new StartBotHandler();
            case ActionNames.ViewMainMenu: return new MainMenuBotHandler();
            case ActionNames.GetAllInfo : return new InfoBotHandler(_disciplineRepository, _render);
        }

        return null;
    }
}