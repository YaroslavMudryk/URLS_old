using URLS.Application.Options;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Subject;

namespace URLS.Application.Services.Interfaces;

public interface ISubjectService
{
    Task<Result<List<SubjectViewModel>>> SearchSubjectsAsync(SearchSubjectOptions options);
    Task<Result<SubjectViewModel>> GetSubjectByIdAsync(int subjectId);
    Task<Result<SubjectViewModel>> GetGroupSubjectAsync(int groupId, int subjectId);
    Task<Result<SubjectViewModel>> CreateSubjectAsync(SubjectCreateModel model);
    Task<Result<SubjectViewModel>> UpdateSubjectAsync(SubjectEditModel model);
}
