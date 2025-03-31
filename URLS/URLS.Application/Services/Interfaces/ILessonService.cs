using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Lesson;

namespace URLS.Application.Services.Interfaces;

public interface ILessonService
{
    Task<Result<List<LessonViewModel>>> GetLessonsBySubjectIdAsync(int subjectId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<Result<LessonViewModel>> GetLessonByIdAsync(long id);
    Task<Result<LessonViewModel>> CreateLessonAsync(LessonCreateModel lesson);
    Task<Result<LessonViewModel>> UpdateLessonAsync(LessonEditModel lesson);
    Task<Result<bool>> RemoveLessonAsync(long id);
}
