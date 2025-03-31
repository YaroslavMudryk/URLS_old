using Microsoft.AspNetCore.Http;

namespace URLS.Constants.Extensions;

public static class HttpContextExtensions
{
    public static string GetLanguage(this HttpContext _httpContext)
    {
        var claims = _httpContext.User.Claims;
        var langClaim = claims.FirstOrDefault(s => s.Type == CustomClaimTypes.Language);
        return langClaim != null ? langClaim.Value : null;
    }
}
