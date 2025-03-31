using URLS.Application.Services.Interfaces;
using URLS.Constants;
using URLS.Domain.Models;
namespace URLS.Application.Extensions;

public static class BaseModelExtensions
{
    public static BaseModel PrepareToCreate(this BaseModel baseModel, IIdentityService identityService = null)
    {
        baseModel.LastUpdatedAt = null;
        baseModel.LastUpdatedBy = null;
        baseModel.LastUpdatedFromIP = null;
        baseModel.CreatedAt = DateTime.Now;
        baseModel.CreatedBy = identityService is null ? Defaults.CreatedBy : identityService.GetIdentityData();
        baseModel.CreatedByUserId = identityService is null ? Defaults.CreatedByUserId : identityService.GetUserId();
        baseModel.CreatedFromIP = identityService is null ? Defaults.IP : identityService.GetIP();
        baseModel.Version = 1;
        return baseModel;
    }

    public static BaseModel PrepareToUpdate(this BaseModel baseModel, IIdentityService identityService = null)
    {
        baseModel.LastUpdatedAt = DateTime.Now;
        baseModel.LastUpdatedBy = identityService is null ? Defaults.CreatedBy : identityService.GetIdentityData();
        baseModel.LastUpdatedByUserId = identityService is null ? Defaults.CreatedByUserId : identityService.GetUserId();
        baseModel.LastUpdatedFromIP = identityService is null ? Defaults.IP : identityService.GetIP();
        baseModel.Version++;
        return baseModel;
    }
}
