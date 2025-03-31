using Microsoft.AspNetCore.Mvc;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Timetable;
using URLS.Constants;
using URLS.Web.Filters;

namespace URLS.Web.Controllers.V1;

[ApiVersion("1.0")]
public class TimetableController : ApiBaseController
{
    private readonly ITimetableService _timetableService;
    public TimetableController(ITimetableService timetableService)
    {
        _timetableService = timetableService;
    }

    [HttpGet]
    [PermissionFilter(PermissionClaims.Timetable, Permissions.CanView)]
    public async Task<IActionResult> GetGroupTimetable(int groupId, DateTime? from, DateTime? to)
    {
        if (from == null)
            from = DateTime.Today;
        if (to == null)
            to = DateTime.Today.AddMonths(1);
        if ((from.HasValue && to.HasValue) && from > to)
        {
            from = DateTime.Today;
            to = DateTime.Today.AddMonths(1);
        }
        return JsonResult(await _timetableService.GetTimetableBetweenDatesAsync(groupId, from.Value, to.Value));
    }

    [HttpPost]
    [PermissionFilter(PermissionClaims.Timetable, Permissions.CanCreate)]
    public async Task<IActionResult> CreateTimetable([FromBody] TimetableCreateModel timetable)
    {
        return JsonResult(await _timetableService.CreateTimetableAsync(timetable));
    }

    [HttpPut]
    [PermissionFilter(PermissionClaims.Timetable, Permissions.CanEdit)]
    public async Task<IActionResult> UpdateTimetable([FromBody] TimetableCreateModel timetable)
    {
        return JsonResult(await _timetableService.UpdateTimetableAsync(timetable));
    }

    [HttpDelete]
    [PermissionFilter(PermissionClaims.Timetable, Permissions.CanRemove)]
    public async Task<IActionResult> DeleteTimetable(long[] ids)
    {
        return JsonResult(await _timetableService.RemoveTimetableAsync(ids));
    }
}
