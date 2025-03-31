using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Export;

namespace URLS.Application.Services.Interfaces;

public interface IExportService
{
    Task<Result<ExportViewModel>> ExportGroupAsync(int groupId);
    Task<Result<ExportViewModel>> ExportLessonMarkAsync(int subjectId, long lessonId);
    Task<Result<ExportViewModel>> ExportMarksBySubjectIdAsync(int subjectId, DateTime from, DateTime to);
    Task<Result<ExportViewModel>> ExportMarksBySubjectIdAsync(int subjectId);
}
