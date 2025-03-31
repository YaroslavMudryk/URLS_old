using URLS.Application.ViewModels.Session;

namespace URLS.Application.Services.Interfaces;

public interface ISessionManager
{
    bool AddSession(TokenModel token);
    bool AddRangeSessions(IEnumerable<TokenModel> tokens);
    bool RemoveSession(string token);
    bool RemoveRangeSession(IEnumerable<string> tokens);
    bool IsActiveSession(string token);
    IList<TokenModel> GetAllTokens();
}
