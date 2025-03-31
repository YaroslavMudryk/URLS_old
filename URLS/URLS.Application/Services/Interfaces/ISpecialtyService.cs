using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Specialty;

namespace URLS.Application.Services.Interfaces;

public interface ISpecialtyService
{
    Task<Result<SpecialtyViewModel>> CreateSpecialtyAsync(SpecialtyCreateModel model);
    Task<Result<SpecialtyViewModel>> UpdateSpecialtyAsync(SpecialtyEditModel model);
    Task<Result<string>> UpdateInviteAsync(int specialtyId);
    Task<Result<string>> GetInviteAsync(int specialtyId);
    Task<Result<SpecialtyViewModel>> GetSpecialtyByIdAsync(int id);
    Task<Result<SpecialtyTeacherViewModel>> CreateSpecialtyTeacherAsync(SpecialtyTeacherCreateModel createModel);
    Task<Result<SpecialtyTeacherViewModel>> UpdateSpecialtyTeacherAsync(SpecialtyTeacherEditModel editModel);
    Task<Result<bool>> RemoveSpecialtyTeacherAsync(int specialtyTeacherId);
    Task<Result<List<SpecialtyTeacherViewModel>>> GetSpecialtyTeachersAsync(int specialtyId, int offset, int count);
    Task<Result<List<SpecialtyViewModel>>> GetAllSpecialtiesAsync();
    Task<Result<List<SpecialtyViewModel>>> GetSpecialtiesByFacultyIdAsync(int facultyId);
}
