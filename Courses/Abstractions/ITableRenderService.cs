using Courses.Models;

namespace Courses.Abstractions;

public interface ITableRenderService
{
    string GetAllDisciplinesInfo(DisciplineModel[] models);
}