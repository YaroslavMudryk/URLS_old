using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLS.Application.Services.Interfaces;

namespace URLS.Web.Controllers.V1;

[ApiVersion("1.0")]
public class WidgetsController : ApiBaseController
{
    private readonly IWidgetService _newsService;
    public WidgetsController(IWidgetService newsService)
    {
        _newsService = newsService;
    }

    [HttpGet("war")]
    [AllowAnonymous]
    public async Task<IActionResult> GetWarInfo(DateTime? date)
    {
        return JsonResult(await _newsService.GetWarInfoAsync(date));
    }

    [HttpGet("covid")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFullWarInfo(DateTime? date)
    {
        return JsonResult(await _newsService.GetCovidInfoAsync(date));
    }
}
