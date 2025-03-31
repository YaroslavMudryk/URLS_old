using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace URLS.Web.Filters;

public class DevelopmentOnlyAttribute : Attribute, IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        var env = context.HttpContext.RequestServices.GetService<IWebHostEnvironment>();
        if (!env.IsProduction())
        {
            context.Result = new NotFoundResult();
        }
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}
