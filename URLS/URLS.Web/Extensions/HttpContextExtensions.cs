using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace URLS.Web.Extensions;

public static class HttpContextExtensions
{
    public static bool IsAuthenticationRequired(this HttpContext httpContext)
    {
        var endpoint = httpContext.GetEndpoint();
        if (endpoint == null)
            return false;
        var metadata = endpoint.Metadata;

        var existAuthAttrbt = metadata.Any(s => s is AuthorizeAttribute);
        if (existAuthAttrbt)
        {
            if (metadata.Any(s => s is AllowAnonymousAttribute))
            {
                return false;
            }
            return true;
        }
        return false;
    }

    public static bool IsPresentPermission(this ClaimsPrincipal user, string type, string value)
    {
        if (!user.Identity.IsAuthenticated)
            return false;

        var claims = user.Claims;

        return claims.Any(s => s.Type == type && s.Value == value);
    }
}
