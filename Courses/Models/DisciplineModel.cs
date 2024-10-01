﻿namespace Courses.Models;

public class DisciplineModel
{
    public string DisciplineName { get; set; } = string.Empty;
    
    public int TotalLectures { get; set; }
    public int TotalPractices { get; set; }

    public List<TopicModel> Topics { get; set; } = new();
}