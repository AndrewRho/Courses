using Courses.Models;

namespace Courses.Abstractions;

public interface IDisciplineRepository
{
    bool IsExists(int id);
    bool IsExists(string name);
    int Create(string name);
    
    // topics will be empty 
    DisciplineModel[] GetList();
    void Delete(string name);
    DisciplineModel[] GetAllWithTopics();
    void UpdateTopics(TopicModel[] topics);
}