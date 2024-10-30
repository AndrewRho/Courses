using System.Text;
using Courses.Abstractions;
using Courses.Configs;
using Courses.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Courses.Controllers;

[Route("telegram")]
public class TelegramController : Controller
{
    private readonly BotConfig _config;
    private readonly IBotHandlerFactory _handlerFactory;
    private readonly ITelegramRestClient _restClient;
    private readonly ICoursesBotContextFactory _contextFactory;

    public TelegramController(
        IOptions<BotConfig> config,
        IBotHandlerFactory handlerFactory,
        ITelegramRestClient restClient,
        ICoursesBotContextFactory contextFactory)
    {
        _handlerFactory = handlerFactory;
        _restClient = restClient;
        _contextFactory = contextFactory;
        _config = config.Value;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> WebHook()
    {
        try
        {
            string body;
            using (var reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
                await LogDebug();
                await LogDebug($"Received webhook data: {body}");
                await LogDebug();
            }

            var update = JsonConvert.DeserializeObject<Update>(body) ?? new Update();
            await LogDebug("Deserialized as:  " + JsonConvert.SerializeObject(update));

            var messageChatId = update.Message?.Chat.Id;
            var callbackChatId = update.CallbackQuery?.Message?.Chat.Id;
            var chatId = messageChatId ?? callbackChatId ?? -1;

            var messageFrom = update.Message?.From?.Id;
            var callbackFrom = update.CallbackQuery?.From.Id;
            var userId = messageFrom ?? callbackFrom ?? -1;

            await LogDebug($"Context: user = {userId}, chat ={chatId}");
            
            if ( userId < 0)
            {
                throw new ArgumentException("User ID is null");
            }

            if (chatId < 0)
            {
                throw new ArgumentException("Chat ID is null");
                
            }

            var context = _contextFactory.GetContext();
            var isUserExists = context.Users.Any(x => x.Id == userId);

            if (!isUserExists)
            {
                await _restClient.SendMessage(chatId, "Ви не авторизовані.", false);
                return Ok();
            }

            var handler = _handlerFactory.GetHandler(update);
            var chatContext = new ChatContext(chatId, userId);

            if (!string.IsNullOrEmpty(update.Message?.Document?.FileId))
            {
                chatContext.FileId = update.Message.Document.FileId;
                await LogDebug($"Received file: {chatContext.FileId}");
            }
            
            
            while (handler != null)
            {
                await handler.Handle(chatContext);
                var nextHandlerType = handler.GetNextHandlerType();
                if (nextHandlerType != null)
                {
                    handler = _handlerFactory.GetHandler(nextHandlerType);
                }
                else
                {
                    break;
                }
            }
        }
        catch (Exception e)
        {
            await LogDebug(e.Message);
        }
        
        return Ok(); 
    }

    private async Task LogDebug(params string[] lines)
    {
        await LogSingleLine(string.Empty);
        foreach (var l in lines)
        {
            await LogSingleLine(l);
        }
    }

    private async Task LogSingleLine(string line)
    {
        Console.WriteLine(line);
        if (_config.IsDebugOutput)
        {
            await _restClient.SendMessage(_config.AdminUserId, line, false);
        }
    }
}