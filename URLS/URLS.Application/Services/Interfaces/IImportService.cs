using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Export;

namespace URLS.Application.Services.Interfaces;

public interface IImportService
{
    Task<Result<ExportViewModel>> ImportNewStudentsAsync(Stream stream);
}
