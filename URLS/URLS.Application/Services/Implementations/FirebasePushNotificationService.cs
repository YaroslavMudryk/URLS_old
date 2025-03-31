using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Firebase;

namespace URLS.Application.Services.Implementations;

public class FirebasePushNotificationService : IPushNotificationService
{
    private readonly List<PendingRequest> _pendingRequests;
    private readonly List<SubscribeUser> _subscribedUsers;
    private readonly FirebaseMessaging _firebaseMessaging;
    private readonly IServiceProvider _serviceProvider;

    public FirebasePushNotificationService(IServiceProvider serviceProvider)
    {
        _pendingRequests = new List<PendingRequest>();
        _subscribedUsers = new List<SubscribeUser>();
        //_firebaseMessaging = GetFirebaseMessaging();  //temporary commented
        _serviceProvider = serviceProvider;
    }

    public async Task<PushResponse> SendPushAsync(int userId, PushMessage pushMessage)
    {
        var user = _subscribedUsers.FirstOrDefault(s => s.UserId == userId);
        if (user == null)
        {
            _pendingRequests.Add(new PendingRequest
            {
                UserId = userId,
                PushMessages = new List<PushMessage> { pushMessage }
            });
            return new PushResponse
            {
                Message = "No devices for sent"
            };
        }

        var listOfMessages = new List<Message>();

        foreach (var device in user.Devices)
        {
            listOfMessages.Add(GetMessage(pushMessage.Title, pushMessage.Body, device.Token));
        }
        var result = await _firebaseMessaging.SendAllAsync(listOfMessages);
        return new PushResponse(result);
    }

    public async Task<PushResponse> SendPushAsync(IEnumerable<int> userIds, PushMessage pushMessage)
    {
        var batchResponse = new List<BatchResponse>();

        foreach (var userId in userIds)
        {
            var user = _subscribedUsers.FirstOrDefault(s => s.UserId == userId);
            if (user == null)
            {
                _pendingRequests.Add(new PendingRequest
                {
                    UserId = userId,
                    PushMessages = new List<PushMessage> { pushMessage }
                });
            }

            var listOfMessages = new List<Message>();

            foreach (var device in user.Devices)
            {
                listOfMessages.Add(GetMessage(pushMessage.Title, pushMessage.Body, device.Token));
            }
            var result = await _firebaseMessaging.SendAllAsync(listOfMessages);
            batchResponse.Add(result);
        }
        return new PushResponse(batchResponse);
    }

    public void Subscribe(SubscribeModel model)
    {
        var identityService = _serviceProvider.GetService<IIdentityService>();
        var user = _subscribedUsers.FirstOrDefault(s => s.UserId == identityService.GetUserId());
        if (user == null)
        {
            var newUser = new SubscribeUser
            {
                UserId = identityService.GetUserId(),
                Email = identityService.GetLoginEmail(),
                Devices = new List<FirebaseDevice>()
            };

            if (model.Type == 1)
                newUser.Devices.Add(FirebaseDevice.AsAndroid(model.Token));
            if (model.Type == 2)
                newUser.Devices.Add(FirebaseDevice.AsIOS(model.Token));
            if (model.Type == 3)
                newUser.Devices.Add(FirebaseDevice.AsWeb(model.Token));

            _subscribedUsers.Add(newUser);
        }
        else
        {
            if (model.Type == 1)
                user.Devices.Add(FirebaseDevice.AsAndroid(model.Token));
            if (model.Type == 2)
                user.Devices.Add(FirebaseDevice.AsIOS(model.Token));
            if (model.Type == 3)
                user.Devices.Add(FirebaseDevice.AsWeb(model.Token));
        }
    }

    public Task SubscribeAsync(SubscribeModel model)
    {
        throw new NotImplementedException();
    }

    public void Unsubscribe(SubscribeModel model)
    {
        var identityService = _serviceProvider.GetService<IIdentityService>();
        var user = _subscribedUsers.FirstOrDefault(s => s.UserId == identityService.GetUserId());
        if (user != null)
        {
            var device = user.Devices.FirstOrDefault(s => s.Token == model.Token);
            if (device != null)
            {
                user.Devices.Remove(device);
                if (user.Devices.Count == 0)
                {
                    _subscribedUsers.Remove(user);
                }
            }
        }
    }

    public Task UnsubscribeAsync(SubscribeModel model)
    {
        throw new NotImplementedException();
    }

    private FirebaseMessaging GetFirebaseMessaging()
    {
        var defaultApp = FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.json"))
        });
        return FirebaseMessaging.GetMessaging(defaultApp);
    }

    private Message GetMessage(string title, string body, string token) //config message for send
    {
        return new Message
        {
            Token = token,
            Notification = new Notification
            {
                Title = title,
                Body = body,
                ImageUrl = ""
            },
            Apns = new ApnsConfig(),
            Android = new AndroidConfig(),
            Webpush = new WebpushConfig(),
        };
    }
}
