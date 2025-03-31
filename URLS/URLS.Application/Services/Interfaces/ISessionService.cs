using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Session;

namespace URLS.Application.Services.Interfaces;

public interface ISessionService
{
    Task<Result<List<SessionViewModel>>> GetAllSessionsByUserIdAsync(int userId, int q = 0, int offset = 0, int limit = 20);
    Task<Result<SessionViewModel>> GetSessionByIdAsync(Guid sessionId);
    Task<Result<bool>> CloseSessionByIdAsync(Guid sessionId);
    Task<Result<bool>> CloseAllSessionsAsync(int userId, bool withCurrent = true);
    Task<Result<bool>> CloseAllSessionsAsync(int userId);
}
