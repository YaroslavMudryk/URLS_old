using Microsoft.AspNetCore.Mvc;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Setting;
using URLS.Constants;
using URLS.Web.Filters;

namespace URLS.Web.Controllers.V1;

[ApiVersion("1.0")]
public class SettingsController : ApiBaseController
{
    private readonly ISettingService _settingService;
    public SettingsController(ISettingService settingService)
    {
        _settingService = settingService;
    }

    [HttpGet]
    [PermissionFilter(PermissionClaims.Settings, Permissions.CanView)]
    public async Task<IActionResult> GetSetting()
    {
        return JsonResult(await _settingService.GetRootSettingAsync());
    }

    [HttpPost]
    [PermissionFilter(PermissionClaims.Settings, Permissions.CanCreate)]
    public async Task<IActionResult> CreateSetting([FromBody] SettingCreateModel model)
    {
        return JsonResult(await _settingService.CreateSettingAsync(model));
    }

    [HttpPut]
    [PermissionFilter(PermissionClaims.Settings, Permissions.CanEdit)]
    public async Task<IActionResult> UpdateSetting([FromBody] SettingEditModel model)
    {
        return JsonResult(await _settingService.UpdateSettingAsync(model));
    }
}
