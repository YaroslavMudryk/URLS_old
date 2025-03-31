using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Setting;
using URLS.Domain.Models;

namespace URLS.Application.Services.Interfaces;

public interface ISettingService
{
    Task<Result<Setting>> GetRootSettingAsync();
    Task<Result<SettingViewModel>> CreateSettingAsync(SettingCreateModel model);
    Task<Result<SettingViewModel>> UpdateSettingAsync(SettingEditModel model);
}
