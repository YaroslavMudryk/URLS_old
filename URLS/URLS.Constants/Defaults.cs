namespace URLS.Constants;

public static class Defaults
{
    public const string Password = "Admin01!";
    public const string CreatedBy = "system";
    public const int CreatedByUserId = 0;
    public const string IP = "::1";
    public const string GroupImage = "https://image.flaticon.com/icons/png/512/25/25437.png";

    public static DateTime GroupInviteActiveFrom = DateTime.Now;
    public static DateTime GroupInviteActiveTo = DateTime.Now.AddMonths(2);

    public const string GroupRegex = @"\w{3}-\d{2}"; //УБД-22

    public const string NeedMFA = "Need MFA";
}
