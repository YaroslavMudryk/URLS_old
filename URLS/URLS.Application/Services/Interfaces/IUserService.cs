using URLS.Application.Options;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.User;
using URLS.Application.ViewModels.User.UserInfo;
using URLS.Domain.Models;

namespace URLS.Application.Services.Interfaces;

public interface IUserService
{
    Task<Result<UserViewModel>> CreateUserAsync(UserCreateModel model);
    Task<Result<List<UserShortViewModel>>> GetLastUsersAsync(int count);
    Task<Result<List<UserShortViewModel>>> GetTeachersAsync(int offset = 0, int count = 20);
    Task<Result<UserViewModel>> GetUserByIdAsync(int id);
    Task<Result<UserFullViewModel>> GetFullInfoUserByIdAsync(int id);
    Task<Result<UserViewModel>> UpdateUsernameAsync(UsernameUpdateModel model);
    Task<Result<List<UserShortViewModel>>> SearchUsersAsync(SearchUserOptions searchUserOptions);
    Task<Result<NotificationSettings>> UpdateNotificationSettingsAsync(int userId, NotificationSettings notificationSettings);
}
