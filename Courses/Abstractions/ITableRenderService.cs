using Courses.Models;

namespace Courses.Abstractions;

public interface ITableRenderService
{
    string GetAllDisciplineInfo(DisciplineModel entity);
}