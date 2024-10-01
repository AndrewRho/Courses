using System.ComponentModel.DataAnnotations;

namespace Courses.Data.Entities;

public class TimeSlotEntity 
{
    [Key]
    public int Id { get; set; }
    public string TimeFrom { get; set; } = string.Empty;
    public string TimeTo { get; set; } = string.Empty;
    
    public string GetSignature()
    {
        return $"{Id}\t{TimeFrom}/{TimeTo}";
    }
}