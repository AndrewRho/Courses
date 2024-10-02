using Courses.Data.Entities;
using Courses.Models;

namespace Courses.Abstractions;

public interface ITableRenderService
{
    string GetAllDisciplineInfo(DisciplineModel entity);
    string GetScheduleInfo(ScheduleEntity[] schedules);
}