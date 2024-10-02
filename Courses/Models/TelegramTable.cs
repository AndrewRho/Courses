using System.Text;

namespace Courses.Models;

public class TelegramTable
{
    private readonly int _cols;
    private readonly List<string[]> _lines = new();

    public TelegramTable(int cols)
    {
        _cols = cols;
    }

    public void Add(params string[] cells)
    {
        var line = new string[_cols];
        for (var i = 0; i < _cols; i++)
        {
            var data = i < cells.Length ? cells[i] : string.Empty;
            line[i] = data;
        }
        
        _lines.Add(line);
    }

    public string GetRenderedText()
    {
        var colSizes = new int[_cols];
        foreach (var l in _lines)
        {
            for (var i = 0; i < _cols; i++)
            {
                if (l[i].Length > colSizes[i])
                {
                    colSizes[i] = l[i].Length;
                }
            }
        }

        var sb = new StringBuilder();
        sb.AppendLine("<pre>");

        foreach (var l in _lines)
        {
            var expectedLength = 0;
            var newLine = string.Empty;
            for (var i = 0; i < _cols; i++)
            {
                newLine += l[i];
                expectedLength += colSizes[i] + 1;

                while (newLine.Length < expectedLength)
                {
                    newLine += " ";
                }
            }

            sb.AppendLine(newLine);
        }
        
        sb.AppendLine("</pre>");
        return sb.ToString();
    }
}