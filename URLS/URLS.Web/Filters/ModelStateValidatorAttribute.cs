using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using URLS.Constants.APIResponse;

namespace URLS.Web.Filters;

public class ModelStateValidatorAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var model = context.ModelState;
        if (!model.IsValid)
        {
            var dict = new Dictionary<string, IList<string>>();
            foreach (var errorItem in model)
            {
                var listErrors = errorItem.Value.Errors;
                foreach (var error in listErrors)
                {
                    if (dict.ContainsKey(errorItem.Key))
                    {
                        dict[errorItem.Key].Add(error.ErrorMessage);
                    }
                    else
                    {
                        dict.Add(errorItem.Key, new List<string> { error.ErrorMessage });
                    }
                }
            }
            context.Result = new BadRequestObjectResult(APIResponse.BadRequestResponse("Not valid", dict));
        }
    }
}
