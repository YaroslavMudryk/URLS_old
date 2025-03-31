using Microsoft.AspNetCore.Http;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Identity;
using URLS.Constants;

namespace URLS.Application.Services.Implementations;

public class HttpIdentityService(
    IHttpContextAccessor httpContextAccessor) : IIdentityService
{
    private HttpContext _httpContext => httpContextAccessor.HttpContext;

    public int GetUserId()
    {
        return _httpContext.GetUserId();
    }

    public string GetUserName()
    {
        return _httpContext.GetUserName();
    }

    public string GetFullName()
    {
        return _httpContext.GetFullName();
    }

    public string GetLoginEmail()
    {
        return _httpContext.GetLoginEmail();
    }

    public Guid GetCurrentSessionId()
    {
        return _httpContext.GetCurrentSessionId();
    }

    public string GetIdentityData()
    {
        return _httpContext.GetIdentityData();
    }

    public string GetBearerToken()
    {
        return _httpContext.GetBearerToken();
    }

    public string GetIP()
    {
        return _httpContext.GetIP();
    }

    public IEnumerable<string> GetRoles()
    {
        return _httpContext.GetRoles();
    }

    public string GetAuthenticationMethod()
    {
        return _httpContext.GetAuthenticationMethod();
    }

    public UserIdentity GetUserDetails()
    {
        return _httpContext.GetUserDetails();
    }

    public IEnumerable<int> GetGroupMemberIds()
    {
        return _httpContext.GetGroupMemberIds();
    }

    public bool IsAdministrator()
    {
        return GetRoles().Any(s => s.Contains(Roles.Admin));
    }
}
