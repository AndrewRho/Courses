using Courses.Models;

namespace Courses.Abstractions;

public interface IScheduleService
{
    Task Process(ChatContext chatContext, string[] lines);
}