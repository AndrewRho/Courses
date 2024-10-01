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
}