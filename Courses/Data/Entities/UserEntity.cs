using System.ComponentModel.DataAnnotations;

namespace Courses.Data.Entities;

public class UserEntity 
{
    [Key]
    public long Id { get; set; }

    public string UserName { get; set; } = string.Empty;
}