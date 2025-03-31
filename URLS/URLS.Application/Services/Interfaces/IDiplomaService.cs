using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Diploma;

namespace URLS.Application.Services.Interfaces;

public interface IDiplomaService
{
    Task<Result<bool>> CreateTemplatesAutomaticallyAsync();
    Task<Result<DiplomaViewModel>> CreateDiplomaBasicOnTemplateAsync(DiplomaCreateModel model, string templateId);
    Task<Result<DiplomaViewModel>> CreateDiplomaTemplateAsync(DiplomaTemplateCreateModel model);
    Task<Result<DiplomaViewModel>> UpdateDiplomaTemplateAsync(DiplomaTemplateEditModel model);
    Task<Result<List<DiplomaViewModel>>> GetDiplomaTemplatesAsync();
    Task<Result<List<DiplomaViewModel>>> GetUserDiplomasAsync(int userId);
    Task<Result<DiplomaViewModel>> GetDiplomaByIdAsync(string id);
    Task<Result<DiplomaViewModel>> GetDiplomaTemplateByIdAsync(string id);
    Task<Result<bool>> RemoveDiplomaAsync(string diplomaId);
}
