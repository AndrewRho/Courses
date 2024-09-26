using Courses.Models;

namespace Courses.Abstractions;

public interface ITopicRepository
{
    bool IsExists(TopicModel model);
    void Add(TopicModel model);
}