namespace Courses.Models;

public class DisciplineModel
{
    public int DisciplineId { get; set; }
    public string DisciplineName { get; set; } = string.Empty;
    public List<TopicModel> Topics { get; set; } = new();
    public int TotalLectures { get; set; }
    public int TotalPractices { get; set; }

    public void CalculateTotals()
    {
        Topics = Topics.OrderBy(x => x.TopicNumber).ToList();
        TotalLectures = Topics.Sum(x => x.Lectures);
        TotalPractices = Topics.Sum(x => x.Practices);
    }
}