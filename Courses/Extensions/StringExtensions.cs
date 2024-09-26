namespace Courses.Extensions;

public static class StringExtensions
{
    // removes double spaces. They often occur in old Word documents.
    public static string Prettify(this string data)
    {
        var split = data.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        return string.Join(" ", split);
    }
}