namespace Courses.Models;

public class ScheduleModel
{
    public int TimeSlotId { get; set; }
    public bool IsLecture { get; set; }
    public string DisciplineName { get; set; } = string.Empty;

    public int DisciplineId { get; set; }
    public DateTime SlotDate { get; set; }
}