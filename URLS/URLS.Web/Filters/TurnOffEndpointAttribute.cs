using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace URLS.Web.Filters;

public class TurnOffEndpointAttribute : Attribute, IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        context.Result = new NotFoundResult();
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}
