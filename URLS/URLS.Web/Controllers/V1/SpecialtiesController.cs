using Microsoft.AspNetCore.Mvc;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Specialty;
using URLS.Constants;
using URLS.Web.Filters;

namespace URLS.Web.Controllers.V1;

[ApiVersion("1.0")]
public class SpecialtiesController : ApiBaseController
{
    private readonly ISpecialtyService _specialtyService;
    private readonly IGroupService _groupService;
    public SpecialtiesController(ISpecialtyService specialtyService, IGroupService groupService)
    {
        _specialtyService = specialtyService;
        _groupService = groupService;
    }

    [HttpGet]
    [PermissionFilter(PermissionClaims.Specialties, Permissions.CanViewAll)]
    public async Task<IActionResult> GetAllSpecialties()
    {
        return JsonResult(await _specialtyService.GetAllSpecialtiesAsync());
    }

    [HttpGet("{id}")]
    [PermissionFilter(PermissionClaims.Specialties, Permissions.CanView)]
    public async Task<IActionResult> GetSpecialtyById(int id)
    {
        return JsonResult(await _specialtyService.GetSpecialtyByIdAsync(id));
    }

    [HttpGet("{id}/invite")]
    [PermissionFilter(PermissionClaims.Specialties, Permissions.CanEdit)]
    public async Task<IActionResult> GetInvite(int id)
    {
        return JsonResult(await _specialtyService.GetInviteAsync(id));
    }

    [HttpPut("{id}/invite")]
    [PermissionFilter(PermissionClaims.Specialties, Permissions.CanEdit)]
    public async Task<IActionResult> UpdateInvite(int id)
    {
        return JsonResult(await _specialtyService.UpdateInviteAsync(id));
    }

    [HttpGet("{id}/groups")]
    [PermissionFilter(PermissionClaims.Specialties, Permissions.CanViewAllGroups)]
    public async Task<IActionResult> GetSpecialtyGroups(int id)
    {
        return JsonResult(await _groupService.GetGroupsBySpecialtyIdAsync(id));
    }

    [HttpPost("teachers")]
    [PermissionFilter(PermissionClaims.Specialties, Permissions.CanCreate)]
    public async Task<IActionResult> CreateSpecialtyTeacher([FromBody] SpecialtyTeacherCreateModel model)
    {
        return JsonResult(await _specialtyService.CreateSpecialtyTeacherAsync(model));
    }

    [HttpPut("teachers/{id}")]
    [PermissionFilter(PermissionClaims.Specialties, Permissions.CanEdit)]
    public async Task<IActionResult> UpdateSpecialtyTeacher(int id, [FromBody] SpecialtyTeacherEditModel model)
    {
        model.Id = id;
        return JsonResult(await _specialtyService.UpdateSpecialtyTeacherAsync(model));
    }

    [HttpDelete("teachers/{id}")]
    [PermissionFilter(PermissionClaims.Specialties, Permissions.CanEdit)]
    public async Task<IActionResult> DeleteSpecialtyTeacher(int id)
    {
        return JsonResult(await _specialtyService.RemoveSpecialtyTeacherAsync(id));
    }

    [HttpGet("{id}/teachers")]
    [PermissionFilter(PermissionClaims.Specialties, Permissions.CanView)]
    public async Task<IActionResult> GetSpecialtyTeachers(int id, int skip = 0, int count = 20)
    {
        return JsonResult(await _specialtyService.GetSpecialtyTeachersAsync(id, skip, count));
    }

    [HttpPost]
    [PermissionFilter(PermissionClaims.Specialties, Permissions.CanCreate)]
    public async Task<IActionResult> CreateSpecialty([FromBody] SpecialtyCreateModel model)
    {
        return JsonResult(await _specialtyService.CreateSpecialtyAsync(model));
    }

    [HttpPut]
    [PermissionFilter(PermissionClaims.Specialties, Permissions.CanEdit)]
    public async Task<IActionResult> UpdateSpecialty([FromBody] SpecialtyEditModel model)
    {
        return JsonResult(await _specialtyService.UpdateSpecialtyAsync(model));
    }
}
