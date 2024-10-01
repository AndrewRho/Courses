namespace Courses.Models;

public class TopicModel
{
    public string TopicName { get; set; } = string.Empty;
    public int TopicNumber { get; set; }
    public int Lectures { get; set; }
    public int Practices { get; set; }
}