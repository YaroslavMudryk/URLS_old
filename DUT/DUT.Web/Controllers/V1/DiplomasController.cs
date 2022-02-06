﻿using DUT.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DUT.Application.ViewModels.Diploma;

namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class DiplomasController : ApiBaseController
    {
        private readonly IDiplomaService _diplomaService;
        private readonly IIdentityService _identityService;
        public DiplomasController(IDiplomaService diplomaService, IIdentityService identityService)
        {
            _diplomaService = diplomaService;
            _identityService = identityService;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyDiplomas()
        {
            return JsonResult(await _diplomaService.GetUserDiplomasAsync(_identityService.GetUserId()));
        }

        [HttpGet("templates")]
        public async Task<IActionResult> GetAllTemplates()
        {
            return JsonResult(await _diplomaService.GetDiplomaTemplatesAsync());
        }

        [HttpPost("templates")]
        public async Task<IActionResult> CreateTemplate([FromBody] DiplomaTemplateCreateModel model)
        {
            return JsonResult(await _diplomaService.CreateDiplomaTemplateAsync(model));
        }

        [HttpPost("templates/{id}")]
        public async Task<IActionResult> CreateDiplomaForStudent(string id, [FromBody] DiplomaCreateModel model)
        {
            return JsonResult(await _diplomaService.CreateDiplomaBasicOnTemplateAsync(model, id));
        }
    }
}