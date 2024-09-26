namespace Courses.Models;

public class TopicModel
{
    public int DisciplineId { get; set; }
    public int TopicNumber { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int Lectures { get; set; }
    public int Practices { get; set; }
}