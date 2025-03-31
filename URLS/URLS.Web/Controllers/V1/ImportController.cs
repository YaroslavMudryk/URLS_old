using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLS.Application.Services.Interfaces;
using URLS.Constants;

namespace URLS.Web.Controllers.V1;

[ApiVersion("1.0")]
public class ImportController : ApiBaseController
{
    private readonly IImportService _importService;
    public ImportController(IImportService importService)
    {
        _importService = importService;
    }

    [HttpPost("students")]
    public async Task<IActionResult> ImportNewStudents(IFormFile file)
    {
        var d = file.OpenReadStream();
        return JsonResult(await _importService.ImportNewStudentsAsync(d));
    }

    [HttpGet("password")]
    [AllowAnonymous]
    public IActionResult GetPass()
    {
        return Ok(Generator.CreateTempPassword());
    }
}
