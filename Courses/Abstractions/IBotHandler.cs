using Courses.Models;

namespace Courses.Abstractions;

public interface IBotHandler
{
    Task Handle(ChatContext context);

    Type? GetNextHandlerType();
}