using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLS.Application.Options;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Lesson;
using URLS.Application.ViewModels.Report;
using URLS.Application.ViewModels.Subject;
using URLS.Domain.Models;

namespace URLS.Web.Controllers.V1;

[ApiVersion("1.0")]
public class SubjectsController : ApiBaseController
{
    private readonly ISubjectService _subjectService;
    private readonly ILessonService _lessonService;
    private readonly IReportService _reportService;
    private readonly IJournalService _journalService;
    private readonly IExportService _exportService;
    public SubjectsController(ISubjectService subjectService, ILessonService lessonService, IReportService reportService, IJournalService journalService, IExportService exportService)
    {
        _subjectService = subjectService;
        _lessonService = lessonService;
        _reportService = reportService;
        _journalService = journalService;
        _exportService = exportService;
    }

    #region Subjects

    [HttpGet]
    public async Task<IActionResult> GetAllSubjects(int offset = 0, int count = 20)
    {
        return JsonResult(await _subjectService.SearchSubjectsAsync(new SearchSubjectOptions
        {
            Offset = offset,
            Count = count
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        return JsonResult(await _subjectService.GetSubjectByIdAsync(id));
    }

    [HttpGet("templates")]
    public async Task<IActionResult> GetTemplateSubjects(int offset, int count)
    {
        return JsonResult(await _subjectService.SearchSubjectsAsync(new SearchSubjectOptions
        {
            IsTemplate = true,
            Offset = offset,
            Count = count
        }));
    }

    [HttpPost]
    public async Task<IActionResult> CreateSubject([FromBody] SubjectCreateModel model)
    {
        return JsonResult(await _subjectService.CreateSubjectAsync(model));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSubject(int id, [FromBody] SubjectEditModel model)
    {
        model.Id = id;
        return JsonResult(await _subjectService.UpdateSubjectAsync(model));
    }

    #endregion

    #region Reports

    [HttpGet("{subjectId}/reports")]
    public async Task<IActionResult> GetAllReports(int subjectId, int offset = 0, int limit = 20)
    {
        return JsonResult(await _reportService.GetReportsBySubjectIdAsync(subjectId, offset, limit));
    }

    [HttpGet("{subjectId}/reports/{id}")]
    public async Task<IActionResult> GetReportById(int subjectId, int id)
    {
        return JsonResult(await _reportService.GetReportIdAsync(subjectId, id));
    }

    [HttpPost("{subjectId}/reports")]
    public async Task<IActionResult> CreateReport(int subjectId)
    {
        return JsonResult(await _reportService.CreateReportAsync(subjectId));
    }

    [HttpPut("{subjectId}/reports/{id}")]
    public async Task<IActionResult> UpdateReport(int subjectId, int id, [FromBody] ReportEditModel model)
    {
        model.Id = id;
        model.SubjectId = subjectId;
        return JsonResult(await _reportService.UpdateReportAsync(model));
    }

    [HttpDelete("{subjectId}/reports/{id}")]
    public async Task<IActionResult> RemoveReport(int subjectId, int id)
    {
        return JsonResult(await _reportService.RemoveReportAsync(subjectId, id));
    }

    #endregion

    #region Lessons

    [HttpGet("{subjectId}/lessons")]
    public async Task<IActionResult> GetSubjectLessons(int subjectId, string from = null, string to = null)
    {
        return JsonResult(await _lessonService.GetLessonsBySubjectIdAsync(subjectId, DateTime.Parse(from), DateTime.Parse(to)));
    }

    [HttpGet("{subjectId}/lessons/export")]
    [AllowAnonymous]
    public async Task<IActionResult> ExportMarks(int subjectId)
    {
        return JsonResult(await _exportService.ExportMarksBySubjectIdAsync(subjectId));
    }

    [HttpGet("{subjectId}/lessons/{lessonId}")]
    public async Task<IActionResult> GetLessonById(int subjectId, long lessonId)
    {
        return JsonResult(await _lessonService.GetLessonByIdAsync(lessonId));
    }

    [HttpGet("{subjectId}/lessons/{lessonId}/export")]
    [AllowAnonymous]
    public async Task<IActionResult> GetExportLesson(int subjectId, long lessonId)
    {
        return JsonResult(await _exportService.ExportLessonMarkAsync(subjectId, lessonId));
    }

    [HttpPost("{subjectId}/lessons")]
    public async Task<IActionResult> CreateLesson(int subjectId, [FromBody] LessonCreateModel model)
    {
        model.SubjectId = subjectId;
        return JsonResult(await _lessonService.CreateLessonAsync(model));
    }

    [HttpPut("{subjectId}/lessons/{lessonId}")]
    public async Task<IActionResult> UpdateLesson(int subjectId, long lessonId, [FromBody]LessonEditModel model)
    {
        model.Id = lessonId;
        model.SubjectId = subjectId;
        return JsonResult(await _lessonService.UpdateLessonAsync(model));
    }

    [HttpDelete("{subjectId}/lessons/{lessonId}")]
    public async Task<IActionResult> RemoveLesson(int subjectId, long lessonId)
    {
        return JsonResult(await _lessonService.RemoveLessonAsync(lessonId));
    }

    #endregion

    #region Journals

    [HttpGet("{subjectId}/journal")]
    public async Task<IActionResult> GetJournal(int subjectId)
    {
        return JsonResult(await _journalService.GetFullJournalAsync(subjectId));
    }

    [HttpGet("{subjectId}/journal/{studentId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStudentJournal(int subjectId, int studentId)
    {
        return JsonResult(await _journalService.GetStudentJournalAsync(subjectId, studentId));
    }

    [HttpGet("{subjectId}/lessons/{lessonId}/journal")]
    public async Task<IActionResult> GetJournalByLesson(int subjectId, long lessonId)
    {
        return JsonResult(await _journalService.GetLessonJournalAsync(subjectId, lessonId));
    }

    [HttpPost("{subjectId}/lessons/{lessonId}/journal")]
    public async Task<IActionResult> CreateJournal(int subjectId, long lessonId)
    {
        return JsonResult(await _journalService.CreateJournalAsync(subjectId, lessonId));
    }

    [HttpPost("{subjectId}/lessons/{lessonId}/journal/synchronize")]
    public async Task<IActionResult> SynchronizeJournal(int subjectId, long lessonId)
    {
        return JsonResult(await _journalService.SynchronizeJournalAsync(subjectId, lessonId));
    }

    [HttpPut("{subjectId}/lessons/{lessonId}/journal")]
    public async Task<IActionResult> UpdateJournal(int subjectId, long lessonId, [FromBody] Journal journal)
    {
        return JsonResult(await _journalService.UpdateJournalAsync(subjectId, lessonId, journal));
    }

    [HttpDelete("{subjectId}/lessons/{lessonId}/journal")]
    public async Task<IActionResult> RemoveJournal(int subjectId, long lessonId)
    {
        return JsonResult(await _journalService.RemoveJournalAsync(subjectId, lessonId));
    }

    #endregion
}
