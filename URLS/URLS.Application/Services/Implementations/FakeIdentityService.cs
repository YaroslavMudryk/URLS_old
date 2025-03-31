using System.Security.Claims;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Identity;
using URLS.Constants;

namespace URLS.Application.Services.Implementations;

public class FakeIdentityService : IIdentityService
{
    public Guid GetCurrentSessionId()
    {
        return new Guid("85204B5B-45C2-4425-A8CE-13198EB289E4");
    }

    public string GetIdentityData()
    {
        return $"{GetLoginEmail()} ({GetUserId()})";
    }

    public string GetFullName()
    {
        return "Мудрик Ярослав";
    }

    public string GetLoginEmail()
    {
        return "yaroslav.mudryk@gmail.com";
    }

    public int GetUserId()
    {
        return 1;
    }

    public string GetUserName()
    {
        return "Yarik08";
    }

    public string GetBearerToken()
    {
        return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
    }

    public string GetIP()
    {
        return "34.43.119.37";
    }

    public IEnumerable<string> GetRoles()
    {
        return new string[] { Roles.Admin }.AsEnumerable();
    }

    public string GetAuthenticationMethod()
    {
        return "pwd";
    }

    public UserIdentity GetUserDetails()
    {
        return new UserIdentity
        {
            CurrentSessionId = GetCurrentSessionId(),
            Fullname = GetFullName(),
            Login = GetLoginEmail(),
            Username = GetUserName(),
            Id = GetUserId(),
            Roles = new string[] { "Administrator", "Developer" },
            Claims = new List<Claim>
            {
                new Claim("University", "Create")
            }
        };
    }

    public IEnumerable<int> GetGroupMemberIds()
    {
        return new int[] { 1 }.AsEnumerable();
    }

    public bool IsAdministrator()
    {
        return false;
    }
}
