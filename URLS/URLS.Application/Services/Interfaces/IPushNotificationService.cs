using URLS.Application.ViewModels.Firebase;

namespace URLS.Application.Services.Interfaces;

public interface IPushNotificationService
{
    void Subscribe(SubscribeModel model);
    Task SubscribeAsync(SubscribeModel model);
    void Unsubscribe(SubscribeModel model);
    Task UnsubscribeAsync(SubscribeModel model);
    Task<PushResponse> SendPushAsync(int userId, PushMessage message);
    Task<PushResponse> SendPushAsync(IEnumerable<int> userIds, PushMessage message);
}
