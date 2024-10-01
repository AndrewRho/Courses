using Newtonsoft.Json;

namespace Courses.Models;

public class GetFileResponse
{
    public bool Ok { get; set; }
    public GetFileResponseResult Result { get; set; } = new();
}

public class GetFileResponseResult
{
    [JsonProperty("file_id")]
    public string FileId { get; set; } = string.Empty;
    
    [JsonProperty("file_unique_id")]
    public string FileUniqueId { get; set; } = string.Empty;

    [JsonProperty("file_size")]
    public int FileSize { get; set; }

    [JsonProperty("file_path")]
    public string FilePath { get; set; } = string.Empty;
}