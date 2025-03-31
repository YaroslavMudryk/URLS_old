using Microsoft.AspNetCore.Mvc;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Firebase;
using URLS.Constants.APIResponse;

namespace URLS.Web.Controllers.V1;

[ApiVersion("1.0")]
public class NotificationsController : ApiBaseController
{
    private readonly INotificationService _notificationsService;
    private readonly IPushNotificationService _pushNotificationService;
    private readonly IIdentityService _identityService;
    public NotificationsController(INotificationService notificationsService, IIdentityService identityService, IPushNotificationService pushNotificationService)
    {
        _notificationsService = notificationsService;
        _identityService = identityService;
        _pushNotificationService = pushNotificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserNotifications(int offset = 0, int count = 20)
    {
        var userId = _identityService.GetUserId();
        return JsonResult(await _notificationsService.GetUserNotificationsAsync(userId, offset, count));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotification(long id)
    {
        return JsonResult(await _notificationsService.GetNotificationByIdAsync(id));
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> ReadNotification(long id)
    {
        return JsonResult(await _notificationsService.ReadNotificationAsync(id));
    }

    [HttpPost("read")]
    public async Task<IActionResult> ReadAllNotificationsAsync()
    {
        return JsonResult(await _notificationsService.ReadAllUserNotificationsAsync(_identityService.GetUserId()));
    }

    [HttpPost("subscribe")]
    public IActionResult Subscribe([FromBody] SubscribeModel model)
    {
        _pushNotificationService.Subscribe(model);
        return Ok(APIResponse.OkResponse());
    }

    [HttpPost("unsubscribe")]
    public IActionResult Unsubscribe([FromBody] SubscribeModel model)
    {
        _pushNotificationService.Unsubscribe(model);
        return Ok(APIResponse.OkResponse());
    }
}
