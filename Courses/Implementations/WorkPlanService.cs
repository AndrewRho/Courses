using Courses.Abstractions;
using Courses.Data.Entities;
using Courses.Models;

namespace Courses.Implementations;

public class WorkPlanService : IWorkPlanService
{
    private readonly ICoursesBotContextFactory _contextFactory;

    public WorkPlanService(ICoursesBotContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task Process(ChatContext chatContext, string[] lines)
    {
        var firstLine = lines.First(x => !string.IsNullOrWhiteSpace(x));
        await chatContext.Say( "Отримано план викладання дисціплини " + firstLine);

        using var dbContext = _contextFactory.GetContext();
        
        var existing = dbContext.Disciplines.FirstOrDefault(x => x.Name == firstLine);
        var topics = GetTopics(lines);

        if (existing == null)
        {
            await chatContext.Say("Це - нова дисціплина, створюю записи");
            existing = new DisciplineEntity
            {
                Name = firstLine
            };

            dbContext.Disciplines.Add(existing);
            await dbContext.SaveChangesAsync();
           
            foreach (var t in topics)
            {
                dbContext.Topics.Add(new TopicEntity
                {
                    Discipline = existing,
                    Name = t.TopicName,
                    Number = t.TopicNumber
                });
            }

            await dbContext.SaveChangesAsync();
        }

        var user = dbContext.Users.Single(x => x.Id == chatContext.UserId);

        foreach (var t in topics)
        {
            var corresponding = dbContext.Topics.FirstOrDefault(x => x.Discipline == existing && x.Name == t.TopicName);
            if (corresponding == null)
            {
                throw new Exception($"Немає теми " + t.TopicName);
            }

            var toDelete = dbContext.WorkPlans.Where(x => x.User == user && x.Topic == corresponding);
            dbContext.WorkPlans.RemoveRange(toDelete);
        }

        await dbContext.SaveChangesAsync();

        foreach (var t in topics)
        {
            var corresponding = dbContext.Topics.First(x => x.Discipline == existing && x.Name == t.TopicName);
            dbContext.WorkPlans.Add(new WorkPlanEntity
            {
                User = user,
                Topic = corresponding,
                Lectures = t.Lectures,
                Practices = t.Practices
            });
        }

        await dbContext.SaveChangesAsync();
    }

   private static TopicModel[] GetTopics(string[] lines)
   {
       var topics = lines
           .Where(x => x.StartsWith("тема", StringComparison.InvariantCultureIgnoreCase))
           .ToArray();

       var models = new List<TopicModel>();

       var ix = 0;
       foreach (var t in topics)
       {
           var tabSplit = t
               .Split('\t')
               .Where( x => !string.IsNullOrWhiteSpace(x))
               .Take(4)
               .ToArray();
           
           ix++;

           var topic = new TopicModel
           {
               TopicName = tabSplit.First(),
               TopicNumber = ix,
               Lectures = GetLowered(tabSplit[2]),
               Practices = GetLowered(tabSplit[3])
           };
           
           models.Add(topic);
       }
       return models.ToArray();
    }
    
    private static int GetLowered(string code)
    {
        // either 6 or 6/4* meaning number of hours decreased due to low number of students
        if (code.Contains('/'))
        {
            return int.Parse(code.Split('/')[1].Split('*')[0]);
        }

        return int.Parse(code);
    }
}