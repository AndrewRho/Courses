using System.ComponentModel.DataAnnotations;

namespace Courses.Data.Entities;

public class DisciplineEntity 
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TopicEntity> Topics { get; set; } = new();
}