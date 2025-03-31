using URLS.Application.ViewModels;
using URLS.Application.ViewModels.University;

namespace URLS.Application.Services.Interfaces;

public interface IUniversityService
{
    Task<Result<UniversityViewModel>> GetUniversityAsync();
    Task<Result<UniversityViewModel>> CreateUniversityAsync(UniversityCreateModel model);
    Task<Result<UniversityViewModel>> UpdateUniversityAsync(UniversityEditModel model);
}
