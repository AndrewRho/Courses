using Courses.Abstractions;
using Courses.Models;

namespace Courses.Implementations;

public class TableRenderService : ITableRenderService
{
    public string GetAllDisciplinesInfo(DisciplineModel[] models)
    {
        var table = new TelegramTable(4);
        table.Add("№", "Назва дисціплини/Теми", "Лекції", "Практики");

        var disciplineIndex = 1;
        foreach (var m in models)
        {
            table.Add();
            table.Add(disciplineIndex.ToString(), m.DisciplineName, m.TotalLectures.ToString(), m.TotalPractices.ToString());

            foreach (var t in m.Topics)
            {
                table.Add($"{disciplineIndex}.{t.TopicNumber}", t.TopicName, t.Lectures.ToString(), t.Practices.ToString());
            }
            disciplineIndex++;
        }

        return table.GetRenderedText();
    }
}