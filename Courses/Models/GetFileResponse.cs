namespace Courses.Models;

public class GetFileResponse
{
    public bool Ok { get; set; }
    public GetFileResponseResult Result { get; set; } = new();
}

public class GetFileResponseResult
{
    public string file_id { get; set; } = string.Empty;
    public string file_unique_id { get; set; } = string.Empty;
    public int file_size { get; set; }
    public string file_path { get; set; } = string.Empty;
}