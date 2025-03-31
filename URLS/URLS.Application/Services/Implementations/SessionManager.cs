using Microsoft.Extensions.DependencyInjection;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Session;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class SessionManager : ISessionManager
{
    private readonly IList<TokenModel> _tokens;

    private IList<TokenModel> GetActualTokensFromDb(URLSDbContext db)
    {
        var sessions = db.Sessions.Where(s => s.IsActive).Select(s => new Session { Token = s.Token, ExpiredAt = s.ExpiredAt }).ToList();
        if (sessions == null || !sessions.Any())
            return new List<TokenModel>();
        return sessions.Select(x => new TokenModel(x.Token, x.ExpiredAt)).ToList();
    }

    public SessionManager(IServiceScopeFactory _serviceScopeFactory)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<URLSDbContext>();
        _tokens = GetActualTokensFromDb(dbContext);
    }

    public bool AddSession(TokenModel token)
    {
        if (!_tokens.Any(x => x.Token == token.Token && x.ExpiredAt == token.ExpiredAt))
        {
            _tokens.Add(token);
        }
        return true;
    }

    public bool AddRangeSessions(IEnumerable<TokenModel> tokens)
    {
        foreach (var token in tokens)
        {
            AddSession(token);
        }
        return true;
    }

    public bool RemoveSession(string token)
    {
        var model = _tokens.FirstOrDefault(x => x.Token == token);
        if (model != null)
            _tokens.Remove(model);
        return true;
    }

    public bool RemoveRangeSession(IEnumerable<string> tokens)
    {
        foreach (var token in tokens)
        {
            RemoveSession(token);
        }
        return true;
    }

    public bool IsActiveSession(string token)
    {
        var tokenModel = _tokens.FirstOrDefault(x => x.Token == token);
        if (tokenModel != null && tokenModel.ExpiredAt > DateTime.Now)
            return true;
        return false;
    }

    public IList<TokenModel> GetAllTokens()
    {
        return _tokens;
    }
}
