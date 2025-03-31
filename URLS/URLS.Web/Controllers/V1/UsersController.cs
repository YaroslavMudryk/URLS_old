using Microsoft.AspNetCore.Mvc;
using URLS.Application.Options;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Identity;
using URLS.Application.ViewModels.User;
using URLS.Constants;
using URLS.Domain.Models;
using URLS.Web.Filters;

namespace URLS.Web.Controllers.V1;

[ApiVersion("1.0")]
public class UsersController : ApiBaseController
{
    private readonly IUserService _userService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IIdentityService _identityService;
    public UsersController(IUserService userService, IIdentityService identityService, IAuthenticationService authenticationService)
    {
        _userService = userService;
        _identityService = identityService;
        _authenticationService = authenticationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLastUsers()
    {
        return JsonResult(await _userService.GetLastUsersAsync(5));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        return JsonResult(await _userService.GetUserByIdAsync(id));
    }

    [HttpGet("{id}/roles")]
    [PermissionFilter(PermissionClaims.Identity, Permissions.All)]
    public async Task<IActionResult> GetUserRoles(int id)
    {
        return JsonResult(await _authenticationService.GetUserRolesAsync(id));
    }

    [HttpPut("{id}/roles")]
    [PermissionFilter(PermissionClaims.Identity, Permissions.All)]
    public async Task<IActionResult> SetupUserRole(int id, [FromBody] UserRoleSetupModel model)
    {
        model.UserId = id;
        return JsonResult(await _authenticationService.SetupUserRolesAsync(model));
    }

    [HttpGet("teachers")]
    public async Task<IActionResult> GetTeachers(int offset = 0, int count = 20)
    {
        return JsonResult(await _userService.GetTeachersAsync(offset, count));
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchUsers([FromBody] SearchUserOptions searchUserOptions)
    {
        return JsonResult(await _userService.SearchUsersAsync(searchUserOptions));
    }

    [HttpPost]
    [PermissionFilter(PermissionClaims.Identity, Permissions.All)]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateModel model)
    {
        return JsonResult(await _userService.CreateUserAsync(model));
    }

    [HttpPatch("username")]
    public async Task<IActionResult> UpdateUsername([FromBody] UsernameUpdateModel model)
    {
        //todo create new permission
        if (model.UserId == null)
            model.UserId = _identityService.GetUserId();
        return JsonResult(await _userService.UpdateUsernameAsync(model));
    }

    [HttpPut("notifications")]
    public async Task<IActionResult> UpdateNotificationSettings([FromBody] NotificationSettings model)
    {
        return JsonResult(await _userService.UpdateNotificationSettingsAsync(_identityService.GetUserId(), model));
    }
}
