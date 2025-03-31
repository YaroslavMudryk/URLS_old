using Microsoft.AspNetCore.Mvc;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Faculty;
using URLS.Constants;
using URLS.Web.Filters;

namespace URLS.Web.Controllers.V1;

[ApiVersion("1.0")]
public class FacultiesController : ApiBaseController
{
    private readonly IFacultyService _facultyService;
    private readonly ISpecialtyService _specialtyService;
    public FacultiesController(IFacultyService facultyService, ISpecialtyService specialtyService)
    {
        _facultyService = facultyService;
        _specialtyService = specialtyService;
    }

    [HttpGet]
    [PermissionFilter(PermissionClaims.Faculties, Permissions.CanViewAll)]
    public async Task<IActionResult> GetFaculties()
    {
        return JsonResult(await _facultyService.GetAllFacultiesAsync());
    }

    [HttpGet("{id}")]
    [PermissionFilter(PermissionClaims.Faculties, Permissions.CanView)]
    public async Task<IActionResult> GetFacultyById(int id)
    {
        return JsonResult(await _facultyService.GetFacultyByIdAsync(id));
    }

    [HttpGet("{facultyId}/specialties")]
    [PermissionFilter(PermissionClaims.Faculties, Permissions.CanViewAllSpecialties)]
    public async Task<IActionResult> GetFacultySpecialties(int facultyId)
    {
        return JsonResult(await _specialtyService.GetSpecialtiesByFacultyIdAsync(facultyId));
    }

    [HttpPost]
    [PermissionFilter(PermissionClaims.Faculties, Permissions.CanCreate)]
    public async Task<IActionResult> CreateFaculty([FromBody] FacultyCreateModel model)
    {
        return JsonResult(await _facultyService.CreateFacultyAsync(model));
    }

    [HttpPut]
    [PermissionFilter(PermissionClaims.Faculties, Permissions.CanEdit)]
    public async Task<IActionResult> UpdateFaculty([FromBody] FacultyEditModel model)
    {
        return JsonResult(await _facultyService.UpdateFacultyAsync(model));
    }
}
