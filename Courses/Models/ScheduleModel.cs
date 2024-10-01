namespace Courses.Models;

public class ScheduleModel
{
    public int TimeSlotId { get; set; }
    public string DisciplineName { get; set; } = string.Empty;
    public int Lectures { get; set; }
    public int Practices { get; set; }
    public DateTime SlotDate { get; set; }
    public Guid DisciplineId { get; set; }

}