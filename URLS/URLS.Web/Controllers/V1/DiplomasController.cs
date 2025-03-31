using Microsoft.AspNetCore.Mvc;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Diploma;
using URLS.Constants;
using URLS.Web.Filters;

namespace URLS.Web.Controllers.V1;

[ApiVersion("1.0")]
public class DiplomasController : ApiBaseController
{
    private readonly IDiplomaService _diplomaService;
    private readonly IIdentityService _identityService;
    public DiplomasController(IDiplomaService diplomaService, IIdentityService identityService)
    {
        _diplomaService = diplomaService;
        _identityService = identityService;
    }

    [HttpGet("{id}")]
    [PermissionFilter(PermissionClaims.Diplomas, Permissions.CanView)]
    public async Task<IActionResult> GetDiplomaById(string id)
    {
        return JsonResult(await _diplomaService.GetDiplomaByIdAsync(id));
    }

    [HttpGet("my")]
    [PermissionFilter(PermissionClaims.Diplomas, Permissions.CanViewAll)]
    public async Task<IActionResult> GetMyDiplomas()
    {
        return JsonResult(await _diplomaService.GetUserDiplomasAsync(_identityService.GetUserId()));
    }

    [HttpGet("templates")]
    [PermissionFilter(PermissionClaims.Diplomas, Permissions.CanViewAll)]
    public async Task<IActionResult> GetAllTemplates()
    {
        return JsonResult(await _diplomaService.GetDiplomaTemplatesAsync());
    }

    [HttpGet("templates/{id}")]
    [PermissionFilter(PermissionClaims.Diplomas, Permissions.CanView)]
    public async Task<IActionResult> GetTemplateById(string id)
    {
        return JsonResult(await _diplomaService.GetDiplomaTemplateByIdAsync(id));
    }

    [HttpPost("templates")]
    [PermissionFilter(PermissionClaims.Diplomas, Permissions.CanCreate)]
    public async Task<IActionResult> CreateTemplate([FromBody] DiplomaTemplateCreateModel model)
    {
        return JsonResult(await _diplomaService.CreateDiplomaTemplateAsync(model));
    }

    [HttpPost("templates/{id}")]
    [PermissionFilter(PermissionClaims.Diplomas, Permissions.CanCreate)]
    public async Task<IActionResult> CreateDiplomaForStudent(string id, [FromBody] DiplomaCreateModel model)
    {
        return JsonResult(await _diplomaService.CreateDiplomaBasicOnTemplateAsync(model, id));
    }
}
