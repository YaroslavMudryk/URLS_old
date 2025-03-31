using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Export;
using URLS.Application.ViewModels.Identity;
using URLS.Constants;
using URLS.Constants.APIResponse;

namespace URLS.Web.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class ApiBaseController : Controller
{
    [NonAction]
    public IActionResult JsonResult<T>(Result<T> result)
    {
        if (typeof(T) == typeof(ExportViewModel))
        {
            var res = result as Result<ExportViewModel>;
            if (res.IsSuccess)
                return File(res.Data.Stream, "Application/msexcel", res.Data.FileName);
        }
        if (result.IsCreated)
            return CreatedResult(result.Data);
        if (result.IsSuccess)
            return Ok(APIResponse.OkResponse(result.Data, result.Meta));
        if (result.IsNotFound)
            return NotFound(APIResponse.NotFoundResponse(result.ErrorMessage));
        if (result.IsError)
        {
            if (result.ErrorMessage == Defaults.NeedMFA && result.Data is JwtToken)
                return BadRequest(APIResponse.BadRequestResponse(Defaults.NeedMFA, (result.Data as JwtToken).SessionId));
            return BadRequest(APIResponse.BadRequestResponse(result.ErrorMessage));
        }
        if (result.IsForbid)
            return JsonForbiddenResult();
        return JsonInternalServerErrorResult();
    }

    [NonAction]
    public IActionResult CreatedResult(object data, Meta meta = null)
    {
        HttpContext.Response.StatusCode = 201;
        return Json(APIResponse.OkResponse(data, meta));
    }

    [NonAction]
    public IActionResult JsonForbiddenResult()
    {
        HttpContext.Response.StatusCode = 403;
        return Json(APIResponse.ForbiddenResposne());
    }

    [NonAction]
    public IActionResult JsonInternalServerErrorResult()
    {
        HttpContext.Response.StatusCode = 500;
        return Json(APIResponse.InternalServerError(HttpContext.TraceIdentifier));
    }
}
