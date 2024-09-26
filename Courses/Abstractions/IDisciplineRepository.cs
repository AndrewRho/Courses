using Courses.Models;

namespace Courses.Abstractions;

public interface IDisciplineRepository
{
    // create
    int Create(string name);

    
    // read
    bool IsExists(int id);
    bool IsExists(string name);

    int GetId(string name);
    // topics will be empty 
    DisciplineModel[] GetList();
    DisciplineModel[] GetAllWithTopics();
    TimeSlotModel[] GetTimeSlots();
    
    // update
    void UpdateTopics(TopicModel[] topics);
    void UpdateSchedule(ScheduleModel[] schedule);
    
    // delete
    void Delete(string name);

}