using System.ComponentModel.DataAnnotations;

namespace Courses.Data.Entities;

public class TopicEntity
{
    [Key]
    public Guid Id { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public DisciplineEntity Discipline { get; set; } = new();

    public List<WorkPlanEntity> WorkPlans { get; set; } = new();
}