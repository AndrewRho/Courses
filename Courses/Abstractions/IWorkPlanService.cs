using Courses.Models;

namespace Courses.Abstractions;

public interface IWorkPlanService
{
    Task Process(ChatContext chatContext, string[] lines);
}