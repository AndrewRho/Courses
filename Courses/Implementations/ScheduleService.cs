using Courses.Abstractions;
using Courses.Data.Entities;
using Courses.Models;
using Microsoft.EntityFrameworkCore;

namespace Courses.Implementations;

public class ScheduleService : IScheduleService
{
    private readonly ICoursesBotContextFactory _contextFactory;
    private readonly ITelegramRestClient _restClient;

    public ScheduleService(ICoursesBotContextFactory contextFactory, ITelegramRestClient restClient)
    {
        _contextFactory = contextFactory;
        _restClient = restClient;
    }

    public async Task ProcessTextFile(ChatContext chatContext, string[] lines)
    {
        await _restClient.SendMessage(chatContext.ChatId, "Отримано розклад занять", false);
        using var dbContext = _contextFactory.GetContext();
        
        var slots = dbContext.TimeSlots.ToArray();
        var curDate = string.Empty;

        var allDisciplinesScheduleModels = new List<ScheduleModel>();
        
        for (var i = 0; i < lines.Length - 3; i++)
        {
            var signature = lines[i] + "/" + lines[i + 1];
            var slot = slots.FirstOrDefault(x => signature.StartsWith(x.GetSignature()));
            if (slot == null)
            {
                continue;
            }

            // first pair, before it date comes
            if (slot.Id == slots.First().Id)
            {
                curDate = lines[i - 1].Split(' ').First();
            }

            if (!lines[i+1].Contains("\t"))
            {
                continue;
            }

            var disciplineName = lines[i + 3];
            var isLecture = disciplineName.EndsWith("(Л)");
            var words = disciplineName.Split(' ');
            disciplineName = string.Join(" ", words.SkipLast(1));
            
            allDisciplinesScheduleModels.Add(new ScheduleModel
            {
                TimeSlotId = slot.Id,
                DisciplineName = disciplineName,
                Lectures = isLecture ? 2 : 0,
                Practices = isLecture ? 0 : 2,
                SlotDate = GetUtc(curDate)
            });
        }

        var uniqueNames = allDisciplinesScheduleModels.Select(x => x.DisciplineName).Distinct().ToArray();

        foreach (var u in uniqueNames)
        {
            var discipline = dbContext.Disciplines.FirstOrDefault(x => x.Name == u); 
            if (discipline == null )
            {
                throw new Exception($"Дисціплина {u} не співпадає з назвою в базі даних");
            }

            foreach (var m in allDisciplinesScheduleModels.Where( x => x.DisciplineName == u))
            {
                m.DisciplineId = discipline.Id;
            }
        }

        var user = dbContext.Users.Single(x => x.Id == chatContext.UserId);
        var userSchedules = dbContext.Schedules.Where(x => x.WorkPlan.User == user).ToArray();
        dbContext.Schedules.RemoveRange(userSchedules);
        await dbContext.SaveChangesAsync();
        
        foreach (var u in uniqueNames)
        {
            var disciplineEntity = dbContext.Disciplines
                .Include( x => x.Topics)
                .ThenInclude( x => x.WorkPlans)
                .Single(x => x.Name == u);

            var scheduleModels = allDisciplinesScheduleModels
                .Where(x => x.DisciplineId == disciplineEntity.Id)
                .ToArray();

            var workPlansExtended = disciplineEntity.Topics
                .SelectMany(x => x.WorkPlans)
                .Select( x => new WorkPlanExtended(x) )
                .ToArray();

            var timeSlots = dbContext.TimeSlots.ToArray();
            
            foreach (var sm in scheduleModels)
            {
                var covers = sm.Practices > 0
                    ? workPlansExtended.FirstOrDefault(x => x.GetPracticesLeft() > 0)
                    : workPlansExtended.FirstOrDefault(x => x.GetLecturesLeft() > 0);

                if (covers == null)
                {
                    throw new Exception("Забагато пар?");
                }

                var timeSlot = timeSlots.Single(x => x.Id == sm.TimeSlotId);
                covers.AddSchedule(sm, timeSlot);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    public ScheduleEntity[] GetSchedule(DateTime dateFrom, DateTime dateTo, long userId)
    {
        using var dbContext = _contextFactory.GetContext();
        var schedules = dbContext.Schedules
            .Include( x=> x.TimeSlot)
            .Include( x => x.WorkPlan)
            .ThenInclude( x => x.Topic)
            .ThenInclude( x => x.Discipline)
            .Where(x => x.WorkPlan.User.Id == userId)
            .Where(x => x.Date >= dateFrom && x.Date <= dateTo)
            .OrderBy( x => x.Date)
            .ThenBy( x => x.TimeSlot.Id)
            .ToArray();

        return schedules;
    }

    private static DateTime GetUtc(string shortDate)
    {
        var split = shortDate.Split('.');
        var day = int.Parse(split[0]);
        var month = int.Parse(split[1]);
        var year = int.Parse(split[2]);

        return new DateTime(year, month, day, 0, 0, 0, 0, DateTimeKind.Utc);
    }

    private class WorkPlanExtended
    {
        private int _originalLectures;
        private int _originalPractices;

        public WorkPlanEntity WorkPlan { get; }

        public int GetLecturesLeft()
        {
            return WorkPlan.Lectures - WorkPlan.Schedules.Sum(x => x.Lectures);
        }

        public int GetPracticesLeft()
        {
            return WorkPlan.Practices - WorkPlan.Schedules.Sum(x => x.Practices);
        }

        public WorkPlanExtended(WorkPlanEntity workPlan)
        {
            WorkPlan = workPlan;
            _originalLectures = workPlan.Lectures;
            _originalPractices = workPlan.Practices;
        }

        public void AddSchedule(ScheduleModel scheduleModel, TimeSlotEntity timeSlotEntity)
        {
            var progress = string.Empty;
            if (scheduleModel.Lectures > 0)
            {
                var totalCovered = _originalLectures - GetLecturesLeft() + 2;
                progress = $"{totalCovered}/{_originalLectures}";
            }

            if (scheduleModel.Practices > 0)
            {
                var totalCovered = _originalPractices - GetPracticesLeft() + 2;
                progress = $"{totalCovered}/{_originalPractices}";
            }
            
            WorkPlan.Schedules.Add(new ScheduleEntity
            {
                Date = scheduleModel.SlotDate,
                Lectures = scheduleModel.Lectures,
                Practices = scheduleModel.Practices,
                TimeSlot = timeSlotEntity,
                Progress = progress
            });
        }
    }
}