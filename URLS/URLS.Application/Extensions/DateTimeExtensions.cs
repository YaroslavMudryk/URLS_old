namespace URLS.Application.Extensions;

public static class DateTimeExtensions
{
    public static DateTime GetStartStudy(this DateTime dateTime)
    {
        return new DateTime(dateTime.Date.Year, 9, 1);
    }

    public static DateTime GetEndStudy(this DateTime dateTime)
    {
        return new DateTime(dateTime.Date.Year + 4, 6, 30);
    }
}
