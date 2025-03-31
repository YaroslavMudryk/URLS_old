using Microsoft.AspNetCore.Mvc;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Apps;
using URLS.Constants;
using URLS.Web.Filters;

namespace URLS.Web.Controllers.V1;

[ApiVersion("1.0")]
public class AppsController : ApiBaseController
{
    private readonly IAppService _appService;
    public AppsController(IAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    [PermissionFilter(PermissionClaims.Apps, Permissions.CanViewAll)]
    public async Task<IActionResult> GetAllApps(int offset = 0, int limit = 20)
    {
        return JsonResult(await _appService.GetAllAppsAsync(offset, limit));
    }

    [HttpGet("{id}")]
    [PermissionFilter(PermissionClaims.Apps, Permissions.CanView)]
    public async Task<IActionResult> GetAppById(int id)
    {
        return JsonResult(await _appService.GetAppByIdAsync(id));
    }

    [HttpGet("{id}/creds")]
    [PermissionFilter(PermissionClaims.Apps, Permissions.CanView)]
    public async Task<IActionResult> GetAppCredsById(int id)
    {
        return JsonResult(await _appService.GetAppDetailsAsync(id));
    }

    [HttpPost]
    [PermissionFilter(PermissionClaims.Apps, Permissions.CanCreate)]
    public async Task<IActionResult> CreateApp([FromBody] AppCreateModel appModel)
    {
        return JsonResult(await _appService.CreateAppAsync(appModel));
    }

    [HttpPut]
    [PermissionFilter(PermissionClaims.Apps, Permissions.CanEdit)]
    public async Task<IActionResult> UpdateApp([FromBody] AppEditModel appModel)
    {
        return JsonResult(await _appService.UpdateAppAsync(appModel));
    }

    [HttpPost("{id}/new-secret")]
    [PermissionFilter(PermissionClaims.Apps, Permissions.CanEdit)]
    public async Task<IActionResult> UpdateApp(int id)
    {
        return JsonResult(await _appService.ChangeAppSecretAsync(id));
    }

    [HttpDelete("{id}")]
    [PermissionFilter(PermissionClaims.Apps, Permissions.CanRemove)]
    public async Task<IActionResult> DeleteApp(int id)
    {
        return JsonResult(await _appService.DeleteAppAsync(id));
    }
}
