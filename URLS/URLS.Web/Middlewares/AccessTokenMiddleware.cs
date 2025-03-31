namespace URLS.Web.Middlewares;

public class AccessTokenMiddleware
{
    private readonly RequestDelegate _next;
    public AccessTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Query["access_token"].ToString();

        if (string.IsNullOrEmpty(context.Request.Headers["Authorization"]) &&
                !string.IsNullOrEmpty(token))
        {
            context.Request.Headers["Authorization"] = $"Bearer {token}";
        }
        await _next(context);
    }
}

public static class AccessTokenMiddlewareExtensions
{
    public static void UseAccessTokenHandler(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<AccessTokenMiddleware>();
    }
}
