using Courses.Data.Entities;
using Courses.Models;

namespace Courses.Abstractions;

public interface IScheduleService
{
    Task ProcessTextFile(ChatContext chatContext, string[] lines);

    ScheduleEntity[] GetSchedule(DateTime dateFrom, DateTime dateTo, long userId);
}