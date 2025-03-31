using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group;
using URLS.Constants;
using URLS.Constants.APIResponse;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class GroupInviteService(
    URLSDbContext db,
    IMapper mapper,
    IIdentityService identityService,
    IPermissionGroupInviteService permissionGroupInviteService) : IGroupInviteService
{
    public async Task<Result<GroupInviteViewModel>> CreateGroupInviteAsync(GroupInviteCreateModel model)
    {
        if(!await permissionGroupInviteService.CanCreateInviteAsync(model.GroupId.Value))
            return Result<GroupInviteViewModel>.Forbiden();

        if (!await db.Groups.AnyAsync(s => s.Id == model.GroupId))
            return Result<GroupInviteViewModel>.NotFound(typeof(Group).NotFoundMessage(model.GroupId));

        if (await db.GroupInvites.AsNoTracking().CountAsync(s => s.GroupId == model.GroupId) >= 5)
            return Result<GroupInviteViewModel>.Error("One group must be have max 5 invites");

        var newGroupInvite = new GroupInvite
        {
            ActiveFrom = model.ActiveFrom,
            ActiveTo = model.ActiveTo,
            Name = model.Name,
            IsActive = model.IsActive,
            CodeJoin = Generator.CreateGroupInviteCode(),
            GroupId = model.GroupId.Value,
        };
        newGroupInvite.PrepareToCreate(identityService);
        await db.GroupInvites.AddAsync(newGroupInvite);
        await db.SaveChangesAsync();
        return Result<GroupInviteViewModel>.Created(mapper.Map<GroupInviteViewModel>(newGroupInvite));
    }

    public async Task<Result<List<GroupInviteViewModel>>> GetGroupInvitesByGroupIdAsync(int groupId)
    {
        if (!await permissionGroupInviteService.CanViewInviteAsync(groupId))
            return Result<List<GroupInviteViewModel>>.Forbiden();

        var groupInvitesFromDb = await db.GroupInvites
            .AsNoTracking()
            .Where(x => x.GroupId == groupId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        var groupInvitesToViews = mapper.Map<List<GroupInviteViewModel>>(groupInvitesFromDb);

        var totalCount = await db.GroupInvites.CountAsync(x => x.GroupId == groupId);

        return Result<List<GroupInviteViewModel>>.SuccessList(groupInvitesToViews, Meta.FromMeta(totalCount, 0, 0));
    }

    public async Task<Result<bool>> RemoveGroupInviteAsync(int groupId, Guid groupInviteId)
    {
        if (!await permissionGroupInviteService.CanRemoveInviteAsync(groupId))
            return Result<bool>.Forbiden();

        var groupInvite = await db.GroupInvites.AsNoTracking().FirstOrDefaultAsync(x => x.Id == groupInviteId);
        if (groupInvite == null)
            return Result<bool>.NotFound(typeof(Group).NotFoundMessage(groupId));

        if (groupInvite.GroupId != groupId)
            return Result<bool>.Error("Incorrect groupId");

        db.GroupInvites.Remove(groupInvite);
        await db.SaveChangesAsync();
        return Result<bool>.Success();
    }

    public async Task<Result<GroupInviteViewModel>> UpdateGroupInviteAsync(GroupInviteEditModel model)
    {
        if(!await permissionGroupInviteService.CanUpdateInviteAsync(model.GroupId.Value))
            return Result<GroupInviteViewModel>.Forbiden();

        var groupInviteFromDb = await db.GroupInvites.FindAsync(model.Id);
        if (groupInviteFromDb == null)
            return Result<GroupInviteViewModel>.NotFound(typeof(Group).NotFoundMessage(model.GroupId));

        groupInviteFromDb.Name = model.Name;
        groupInviteFromDb.ActiveFrom = model.ActiveFrom;
        groupInviteFromDb.ActiveTo = model.ActiveTo;
        groupInviteFromDb.IsActive = model.IsActive;
        groupInviteFromDb.PrepareToUpdate(identityService);

        db.GroupInvites.Update(groupInviteFromDb);
        await db.SaveChangesAsync();

        return Result<GroupInviteViewModel>.SuccessWithData(mapper.Map<GroupInviteViewModel>(groupInviteFromDb));
    }
}
