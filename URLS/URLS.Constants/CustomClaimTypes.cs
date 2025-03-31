using System.Security.Claims;

namespace URLS.Constants;

public static class CustomClaimTypes
{
    public const string CurrentSessionId = "SessionId";
    public const string GroupMemberId = "MemberId";
    public const string UserId = ClaimTypes.NameIdentifier;
    public const string UserName = ClaimTypes.Name;
    public const string FullName = ClaimTypes.GivenName;
    public const string Login = "Login";
    public const string AuthenticationMethod = ClaimTypes.AuthenticationMethod;
    public const string Role = ClaimTypes.Role;
    public const string Language = "Language";
}
