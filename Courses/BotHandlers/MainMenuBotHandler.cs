using Courses.Abstractions;
using Courses.Models;

namespace Courses.BotHandlers;

public class MainMenuBotHandler : BotHandlerBase
{
    private readonly ITelegramRestClient _restClient;
    
    public MainMenuBotHandler(ITelegramRestClient client) : base(client)
    {
        _restClient = client;
    }

    protected override async Task HandleSafe(ChatContext context)
    {
        await _restClient.SendStartMenuButtons(context.ChatId);
    }
}