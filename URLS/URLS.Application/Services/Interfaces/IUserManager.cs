using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Identity;
using URLS.Application.ViewModels.User;
using URLS.Domain.Models;

namespace URLS.Application.Services.Interfaces;

public interface IUserManager
{
    Task<Result<User>> GetUserByLoginAsync(string login);
    Task<Result<User>> GetUserByIdAsync(int id);
    Task<Result<User>> LoginAsync(string login, string password);
    Task<Result<User>> LoginByExternalProviderAsync(ExternalProviderInfo externalProvider);
    Task<Result<User>> RegisterAsync(RegisterViewModel registerModel);
    Task<Result<UserLogin>> LinkExternalProviderToCurrentUserAsync(ExternalProviderInfo externalProvider, int userId);
    Task<Result<List<UserLogin>>> GetExternalProvidersByUserAsync(int id);
    Task<Result<(bool, DateTime?)>> IsBlockUserAsync(int id);
    Task<Result<DateTime>> BlockUserAsync(User user, DateTime? blockUntil = null);
    Task<Result<User>> UnBlockUserAsync(int id);
    Task<Result<User>> IncrementAccessFailedAndBlockAsync(int id);
}
