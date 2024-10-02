using Courses.Abstractions;
using Courses.Data.Entities;
using Courses.Models;

namespace Courses.Implementations;

public class TableRenderService : ITableRenderService
{
    public string GetAllDisciplineInfo(DisciplineModel model)
    {
        var table = new TelegramTable(4);
        table.Add("№", "Назва дисціплини/Теми", "Лекції", "Практики");

        table.Add();
        table.Add(string.Empty, model.DisciplineName, model.TotalLectures.ToString(), model.TotalPractices.ToString());

        foreach (var t in model.Topics)
        {
            table.Add($"{t.TopicNumber}", t.TopicName, t.Lectures.ToString(), t.Practices.ToString());
        }

        return table.GetRenderedText();
    }

    public string GetScheduleInfo(ScheduleEntity[] schedules)
    {
        var table = new TelegramTable(3);

        table.Add("№", "Час", "Дисціплина");
        table.Add(string.Empty, "Тип", "Тема");
        table.Add();
        foreach (var s in schedules)
        {
            table.Add(s.TimeSlot.Id.ToString(), s.TimeSlot.TimeFrom + " - " + s.TimeSlot.TimeTo, s.WorkPlan.Topic.Discipline.Name );
            table.Add(string.Empty, ( s.Lectures > 0 ? "Лекція " : "Практика " ) + s.Progress , s.WorkPlan.Topic.Name );
            table.Add();
        }

        return table.GetRenderedText();
    }
}