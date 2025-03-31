using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLS.Application.Seeder;

namespace URLS.Web.Controllers.V1;

[ApiVersion("1.0")]
public class NewController : ApiBaseController
{
    private readonly ISeederService _seederService;
    public NewController(ISeederService seederService)
    {
        _seederService = seederService;
    }


    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> InitSystem()
    {
        return JsonResult(await _seederService.SeedSystemAsync());
    }
}
