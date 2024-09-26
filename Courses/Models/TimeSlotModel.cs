namespace Courses.Models;

public class TimeSlotModel
{
    public int Id { get; set; }
    public string TimeFrom { get; set; } = string.Empty;
    public string TimeTo { get; set; } = string.Empty;

    public string GetSignature()
    {
        return $"{Id}\t{TimeFrom}/{TimeTo}";
    }
}