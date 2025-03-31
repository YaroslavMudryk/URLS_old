using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Notification;
using URLS.Domain.Models;

namespace URLS.Application.Services.Interfaces;

public interface INotificationService
{
    Task<Result<List<NotificationViewModel>>> GetUserNotificationsAsync(int userId, int offset, int count);
    Task<Result<NotificationViewModel>> GetNotificationByIdAsync(long notifyId);
    Task<Result<NotificationViewModel>> ReadNotificationAsync(long notifyId);
    Task<Result<bool>> ReadAllUserNotificationsAsync(int userId);
    Task<Result<bool>> SendNotifyToUsersAsync(Notification notification, IEnumerable<int> userIds);
    Task<Result<bool>> SendNotifyToUserAsync(Notification notification, int userId);
}
