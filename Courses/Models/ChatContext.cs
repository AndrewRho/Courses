using Telegram.Bot;
using Telegram.Bot.Types;

namespace Courses.Models;

public class ChatContext
{
    public ITelegramBotClient Client { get;  }
    public ChatId ChatId { get;  }
    public long UserId { get; }
    public CancellationToken CancelToken { get; }
    public string FileId { get; set; } = string.Empty;

    public ChatContext(ITelegramBotClient client, ChatId chatId, long userId, CancellationToken cancelToken)
    {
        Client = client;
        ChatId = chatId;
        CancelToken = cancelToken;
        UserId = userId;
    }

    public async Task Say(string what)
    {
        await Client.SendTextMessageAsync(ChatId, what, cancellationToken: CancelToken);
    }
}