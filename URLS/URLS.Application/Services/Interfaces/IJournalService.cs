using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Lesson;
using URLS.Domain.Models;

namespace URLS.Application.Services.Interfaces;

public interface IJournalService
{
    Task<Result<LessonViewModel>> CreateJournalAsync(int subjectId, long lessonId);
    Task<Result<LessonViewModel>> UpdateJournalAsync(int subjectId, long lessonId, Journal journal);
    Task<Result<LessonViewModel>> RemoveJournalAsync(int subjectId, long lessonId);
    Task<Result<LessonViewModel>> SynchronizeJournalAsync(int subjectId, long lessonId);
    Task<Result<Journal>> GetLessonJournalAsync(int subjectId, long lessonId);
    Task<Result<FullJournalViewModel>> GetFullJournalAsync(int subjectId);
    Task<Result<FullJournalViewModel>> GetStudentJournalAsync(int subjectId, int userId);
}
