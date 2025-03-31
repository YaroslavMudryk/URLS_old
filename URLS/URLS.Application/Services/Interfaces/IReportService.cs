using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Report;

namespace URLS.Application.Services.Interfaces;

public interface IReportService
{
    Task<Result<ReportViewModel>> CreateReportAsync(int subjectId);
    Task<Result<List<ReportViewModel>>> GetReportsBySubjectIdAsync(int subjectId, int offset = 0, int limit = 20);
    Task<Result<ReportViewModel>> UpdateReportAsync(ReportEditModel model);
    Task<Result<ReportViewModel>> GetReportIdAsync(int subjectId, int id);
    Task<Result<bool>> RemoveReportAsync(int subjectId, int id);
}
