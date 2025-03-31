using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Session;
using URLS.Constants;
using URLS.Constants.APIResponse;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class SessionService(
    URLSDbContext db,
    IMapper mapper,
    IIdentityService identityService,
    ISessionManager sessionManager,
    ICommonService commonService) : ISessionService
{
    public async Task<Result<SessionViewModel>> GetSessionByIdAsync(Guid sessionId)
    {
        var session = await db.Sessions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == sessionId);
        if (session == null)
            return Result<SessionViewModel>.NotFound("Session not found");

        if (session.UserId != identityService.GetUserId())
            if (!identityService.GetRoles().Contains(Roles.Admin))
                return Result<SessionViewModel>.Error("Access denited");

        return Result<SessionViewModel>.SuccessWithData(mapper.Map<SessionViewModel>(session));
    }

    public async Task<Result<List<SessionViewModel>>> GetAllSessionsByUserIdAsync(int userId, int q = 0, int offset = 0, int limit = 20)
    {
        if (userId != identityService.GetUserId())
            if (!identityService.GetRoles().Contains(Roles.Admin))
                return Result<List<SessionViewModel>>.Error("Access denited");

        if (offset < 0 || limit < 0 && (q != 0 || q != 1 || q != 2))
            return Result<List<SessionViewModel>>.Error("Please check enter data");

        var query = db.Sessions
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Skip(offset).Take(limit);

        if (q == 0)
            query = query.Where(x => x.IsActive);
        if (q == 1)
            query = query.Where(x => !x.IsActive);

        var sessions = await query.ToListAsync();

        var sessionsToView = SortSessions(sessions);

        var totalCount = await commonService
            .CountAsync<Session>(x => x.UserId == userId && q == 0 ? x.IsActive : !x.IsActive);

        return Result<List<SessionViewModel>>.SuccessList(sessionsToView, Meta.FromMeta(totalCount, offset, limit));
    }

    private List<SessionViewModel> SortSessions(List<Session> sessions)
    {
        var sortedSessions = new List<SessionViewModel>();
        if (sessions == null || sessions.Count == 0)
            return sortedSessions;

        var activeSessions = sessions.Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedAt).Select(x => new SessionViewModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                IsActive = x.IsActive,
                App = x.App,
                Client = x.Client,
                DeactivatedAt = x.DeactivatedAt,
                Location = x.Location
            });
        sortedSessions.AddRange(activeSessions);

        var unActiveSessions = sessions.Where(s => !s.IsActive)
            .OrderByDescending(x => x.DeactivatedAt).Select(x => new SessionViewModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                IsActive = x.IsActive,
                App = x.App,
                Client = x.Client,
                DeactivatedAt = x.DeactivatedAt,
                Location = x.Location
            });
        sortedSessions.AddRange(unActiveSessions);

        var currentToken = identityService.GetBearerToken();
        var currentSessionId = sessions.FirstOrDefault(s => s.Token == currentToken)?.Id;

        for (int i = 0; i < sortedSessions.Count; i++)
        {
            if (currentSessionId != null)
                if (sortedSessions[i].Id == currentSessionId)
                {
                    sortedSessions[i].IsCurrent = true;
                }
        }
        var currentSession = sortedSessions.FirstOrDefault(x => x.IsCurrent);
        sortedSessions.Remove(currentSession);
        sortedSessions.Insert(0, currentSession);
        return sortedSessions;
    }

    public async Task<Result<bool>> CloseSessionByIdAsync(Guid sessionId)
    {
        var session = await db.Sessions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == sessionId);
        if (session == null)
            return Result<bool>.NotFound("Session not found");

        if (session.UserId != identityService.GetUserId())
            if (identityService.GetRoles().Any(s => s == Roles.Admin))
                return Result<bool>.Error("Access denited");

        if (!session.IsActive && !sessionManager.IsActiveSession(session.Token))
            return Result<bool>.Error("Session is already closed");

        sessionManager.RemoveSession(session.Token);
        session.IsActive = false;
        session.DeactivatedAt = DateTime.Now;
        session.DeactivatedBySessionId = identityService.GetCurrentSessionId();
        session.PrepareToUpdate(identityService);
        db.Sessions.Update(session);
        await db.SaveChangesAsync();
        return Result<bool>.Success();
    }

    public async Task<Result<bool>> CloseAllSessionsAsync(int userId, bool withCurrent = true)
    {
        var currentUserId = identityService.GetUserId();

        if (userId != currentUserId)
            if (identityService.GetRoles().Any(s => s == Roles.Admin))
                return Result<bool>.Error("Access denited");

        var sessionsToClose = await db.Sessions.AsNoTracking().Where(x => x.IsActive && x.UserId == userId).ToListAsync();

        if (sessionsToClose == null || sessionsToClose.Count == 0)
            return Result<bool>.Success();

        var currentToken = identityService.GetBearerToken();
        var now = DateTime.Now;
        var currentSessionId = identityService.GetCurrentSessionId();

        if (!withCurrent)
            sessionsToClose.Remove(sessionsToClose.FirstOrDefault(s => s.Token == currentToken));

        sessionsToClose.ForEach(x =>
        {
            x.IsActive = false;
            x.DeactivatedAt = now;
            x.DeactivatedBySessionId = currentSessionId;
            x.PrepareToUpdate(identityService);
        });

        sessionManager.RemoveRangeSession(sessionsToClose.Select(x => x.Token));
        db.Sessions.UpdateRange(sessionsToClose);
        await db.SaveChangesAsync();
        return Result<bool>.Success();
    }

    public async Task<Result<bool>> CloseAllSessionsAsync(int userId)
    {
        var sessionsToClose = await db.Sessions.AsNoTracking().Where(s => s.UserId == userId && s.IsActive).ToListAsync();

        if (sessionsToClose == null || sessionsToClose.Count == 0)
            return Result<bool>.Success();

        var now = DateTime.Now;

        sessionsToClose.ForEach(x =>
        {
            x.IsActive = false;
            x.DeactivatedAt = now;
            x.DeactivatedBySessionId = identityService != null ? identityService.GetCurrentSessionId() : null;
            x.PrepareToUpdate(identityService);
        });

        sessionManager.RemoveRangeSession(sessionsToClose.Select(x => x.Token));
        db.Sessions.UpdateRange(sessionsToClose);
        await db.SaveChangesAsync();
        return Result<bool>.Success();
    }
}
