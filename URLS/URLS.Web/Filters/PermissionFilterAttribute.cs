using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using URLS.Constants.APIResponse;
using URLS.Web.Extensions;

namespace URLS.Web.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PermissionFilterAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _type;
    private readonly string _value;

    public PermissionFilterAttribute(string type, string value)
    {
        _type = type;
        _value = value;
    }

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if(!context.HttpContext.User.Identity.IsAuthenticated)
            return Task.CompletedTask;
        var currentUser = context.HttpContext.User;
        if (!currentUser.IsPresentPermission(_type, _value))
        {
            context.HttpContext.Response.StatusCode = 403;
            context.Result = new JsonResult(APIResponse.ForbiddenResposne());
        }
        return Task.CompletedTask;
    }
}
