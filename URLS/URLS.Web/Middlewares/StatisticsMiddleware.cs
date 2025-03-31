using URLS.Application.Extensions;

namespace URLS.Web.Middlewares;

public class StatisticsMiddleware
{
    private readonly ILogger<StatisticsMiddleware> _logger;
    private readonly RequestDelegate _next;
    public StatisticsMiddleware(RequestDelegate next, ILogger<StatisticsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }


    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation(GetLogMessage(context));
        await _next(context);
    }

    private string GetLogMessage(HttpContext context)
    {
        var path = context.Request.Path.Value;
        var requestId = context.TraceIdentifier;
        var request = context.Request;
        return $"({requestId}|{path}({request.Method})|{GetUserInfo(context)})";
    }

    private string GetUserInfo(HttpContext context)
    {
        if (!context.User.Identity.IsAuthenticated)
            return $"{context.Connection.RemoteIpAddress.ToString()}";
        var user = context.GetUserDetails();
        return $"{user.Id}({user.Login})|{user.CurrentSessionId}";
    }
}

public static class StatisticsMiddlewareExtensions
{
    public static void UseStatistics(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<StatisticsMiddleware>();
    }
}
