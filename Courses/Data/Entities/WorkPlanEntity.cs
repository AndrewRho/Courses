using System.ComponentModel.DataAnnotations;

namespace Courses.Data.Entities;

public class WorkPlanEntity
{
    [Key]
    public Guid Id { get; set; }
    public UserEntity User { get; set; } = new();
    public TopicEntity Topic { get; set; } = new();
    public List<ScheduleEntity> Schedules { get; set; } = new();
    public int Lectures { get; set; }
    public int Practices { get; set; }
}