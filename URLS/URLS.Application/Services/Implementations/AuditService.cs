using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Audit;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class AuditService(
    URLSDbContext db,
    IIdentityService identityService) : IAuditService
{
    public async Task<Result<bool>> CreateAuditAsync(AuditCreateModel model)
    {
        var audit = new Audit
        {
            Entity = model.Entity,
            EntityId = model.EntityId,
            Before = model.Before != null ? JsonSerializer.Serialize(model.Before) : null,
            After = model.Before != null ? JsonSerializer.Serialize(model.After) : null
        };

        audit.PrepareToCreate(identityService);
        await db.Audits.AddAsync(audit);
        await db.SaveChangesAsync();
        return Result<bool>.Success();
    }

    public async Task<Result<AuditViewModel<T>>> GetAuditByIdAsync<T>(long id)
    {
        var res = await db.Audits
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (res == null)
            return Result<AuditViewModel<T>>.NotFound(typeof(Audit).NotFoundMessage(id));

        var auditViewModel = new AuditViewModel<T>
        {
            Id = id,
            Entity = res.Entity,
            Before = JsonSerializer.Deserialize<T>(res.Before),
            After = JsonSerializer.Deserialize<T>(res.After),
            CreatedAt = res.CreatedAt,
            EntityId = res.EntityId,
        };

        return Result<AuditViewModel<T>>.SuccessWithData(auditViewModel);
    }

    public async Task<Result<List<AuditViewModel<T>>>> GetAuditsByItemIdAsync<T>(string id, string entity)
    {
        var items = await db.Audits
            .AsNoTracking()
            .Where(s => s.EntityId == id && s.Entity == entity)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        var audits = items.Select(audit => new AuditViewModel<T>
        {
            Id = audit.Id,
            CreatedAt = audit.CreatedAt,
            Entity = audit.Entity,
            EntityId = audit.EntityId,
            Before = JsonSerializer.Deserialize<T>(audit.Before),
            After = JsonSerializer.Deserialize<T>(audit.After)
        }).ToList();

        return Result<List<AuditViewModel<T>>>.SuccessWithData(audits);
    }
}
