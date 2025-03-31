namespace URLS.Application.Extensions;

public static class StringExtensions
{
    public static string GetNumericQuestion(this string text, int number)
    {
        return $"{number}. {text}";
    }

    public static string GetNumericAnswer(this string text, int number)
    {
        return $"{number}) {text}";
    }

    public static int GetIndexForGroup(this string groupName)
    {
        if (groupName.Contains('-'))
        {
            var indexNumber = groupName.IndexOf("-");
            return indexNumber + 1;
        }
        return -1;
    }
}
