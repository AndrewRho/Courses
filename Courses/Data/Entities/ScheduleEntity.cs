using System.ComponentModel.DataAnnotations;

namespace Courses.Data.Entities;

public class ScheduleEntity
{
    [Key]
    public Guid Id { get; set; }

    public WorkPlanEntity WorkPlan { get; set; } = new();
    public TimeSlotEntity TimeSlot { get; set; } = new();
    public DateTime Date { get; set; }
    public int Lectures { get; set; }
    public int Practices { get; set; }
}