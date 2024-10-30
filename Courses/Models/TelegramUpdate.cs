using Newtonsoft.Json;

namespace Courses.Models;

public class TelegramUpdate
{
    [JsonProperty("update_id")]
    public int UpdateId { get; set; }
    public Message Message { get; set; } = new();

    [JsonProperty("callback_query")]
    public Message Callback { get; set; } = new();
}

public class Message
{
    [JsonProperty("message_id")]
    public int MessageId { get; set; }
    
    public User From { get; set; } = new();
    public Chat Chat { get; set; } = new();
    public long Date { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class User
{
    public int Id { get; set; }
    
    [JsonProperty("is_bot")]
    public bool IsBot { get; set; }
    
    [JsonProperty("first_name")]
    public string FirstName { get; set; } = string.Empty;
    
    [JsonProperty("last_name")]
    public string LastName { get; set; } = string.Empty;
    
    public string Username { get; set; } = string.Empty;
    
    [JsonProperty("language_code")]
    public string LanguageCode { get; set; } = string.Empty;
}

public class Chat
{
    public long Id { get; set; }

    [JsonProperty("first_name")]
    public string FirstName { get; set; } = string.Empty;
    
    [JsonProperty("last_name")]
    public string LastName { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}