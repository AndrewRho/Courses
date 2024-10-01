using Courses.Data;

namespace Courses.Abstractions;

public interface ICoursesBotContextFactory
{
    CoursesBotContext GetContext();
}