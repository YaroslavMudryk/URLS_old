using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group.GroupMember;
using URLS.Application.ViewModels.User;
using URLS.Constants.APIResponse;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class GroupMemberService(
    URLSDbContext db,
    IIdentityService identityService,
    ICommonService commonService,
    ISessionService sessionService,
    ISessionManager sessionManager) : IGroupMemberService
{
    public async Task<Result<bool>> AcceptAllNewGroupMembersAsync(int groupId)
    {
        if (!await CanAcceptOrRejectNewJoinersAsync(groupId))
            return Result<bool>.Forbiden();

        var allNewGroupMembers = await db.UserGroups
            .AsNoTracking()
            .Where(s => s.Status == UserGroupStatus.New && s.GroupId == groupId)
            .Include(s => s.User)
            .ToListAsync();

        if (allNewGroupMembers == null || allNewGroupMembers.Count == 0)
            return Result<bool>.Success();

        allNewGroupMembers.ForEach(gm =>
        {
            gm.Status = UserGroupStatus.Member;
            gm.PrepareToUpdate(identityService);
            gm.User.IsActivateAccount = true;
        });

        db.UserGroups.UpdateRange(allNewGroupMembers);
        await db.SaveChangesAsync();
        return Result<bool>.Success();
    }

    public async Task<Result<bool>> AcceptNewGroupMemberAsync(int groupId, int groupMemberId, UserEditModel userModel)
    {
        if (!await CanAcceptOrRejectNewJoinersAsync(groupId))
            return Result<bool>.Forbiden();

        var newGroupMember = await db.UserGroups
            .AsNoTracking()
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == groupMemberId);

        if (newGroupMember == null)
            return Result<bool>.NotFound(typeof(UserGroup).NotFoundMessage(groupMemberId));

        if (newGroupMember.GroupId != groupId)
            return Result<bool>.Error("Member not from current group");

        if (newGroupMember.Status == UserGroupStatus.Member)
            return Result<bool>.Error("User is already member of group");

        var newUser = newGroupMember.User;
        if (userModel != null)
        {
            newUser.FirstName = userModel.FirstName;
            newUser.LastName = userModel.LastName;
            newUser.MiddleName = userModel.MiddleName;
        }
        newUser.IsActivateAccount = true;
        newUser.PrepareToUpdate(identityService);

        newGroupMember.Status = UserGroupStatus.Member;
        newGroupMember.PrepareToUpdate(identityService);

        db.UserGroups.Update(newGroupMember);
        await db.SaveChangesAsync();

        return Result<bool>.Success();
    }

    public async Task<Result<GroupMemberViewModel>> GetGroupMemberByIdAsync(int groupId, int memberId)
    {
        if (!await commonService.IsExistAsync<Group>(x => x.Id == groupId))
            return Result<GroupMemberViewModel>.NotFound(typeof(Group).NotFoundMessage(groupId));

        var groupMember = await db.UserGroups
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.UserGroupRole)
            .FirstOrDefaultAsync(x => x.Id == memberId);

        if (groupMember == null)
            return Result<GroupMemberViewModel>.NotFound($"Member with ID {memberId} not found");

        if (groupMember.GroupId != groupId)
            return Result<GroupMemberViewModel>.Error("Group don't have this member");

        var groupMemberToView = groupMember.MapToView();
        return Result<GroupMemberViewModel>.SuccessWithData(groupMemberToView);
    }

    public async Task<Result<List<GroupMemberViewModel>>> GetGroupMembersAsync(int groupId, int offset = 0, int count = 20, int status = 0)
    {
        if (!await commonService.IsExistAsync<Group>(x => x.Id == groupId))
            return Result<List<GroupMemberViewModel>>.NotFound(typeof(Group).NotFoundMessage(groupId));

        var userGroupRoles = new List<UserGroupRole>();

        var query = db.UserGroups
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.GroupId == groupId)
            .OrderByDescending(x => x.Id)
            .Skip(offset).Take(count);

        if (status > 0 && status < 4)
        {
            query = query.Where(x => x.Status == (UserGroupStatus)status);
        }

        var groupMembers = await query.ToListAsync();

        foreach (var groupMember in groupMembers)
        {
            var userGroupRole = userGroupRoles.FirstOrDefault(s => s.Id == groupMember.UserGroupRoleId);
            if (userGroupRole == null)
            {
                var currentUserGroupRole = await db.UserGroupRoles.AsNoTracking().FirstOrDefaultAsync(s => s.Id == groupMember.UserGroupRoleId);
                userGroupRoles.Add(currentUserGroupRole);
                groupMember.UserGroupRole = currentUserGroupRole;
            }
            else
            {
                groupMember.UserGroupRole = userGroupRole;
            }
        }

        if (groupMembers == null)
            return Result<List<GroupMemberViewModel>>.Success();

        var totalCount = await db.UserGroups.CountAsync(x => x.GroupId == groupId);

        var groupMembersToView = groupMembers.MapToViews(false);
        return Result<List<GroupMemberViewModel>>.SuccessList(groupMembersToView, Meta.FromMeta(totalCount, offset, count));
    }

    public async Task<Result<bool>> RejectNewGroupMemberAsync(int groupId, int groupMemberId)
    {
        if (!await CanAcceptOrRejectNewJoinersAsync(groupId))
            return Result<bool>.Forbiden();

        var groupMember = await db.UserGroups.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == groupMemberId && s.Status == UserGroupStatus.New);
        if (groupMember == null)
            return Result<bool>.NotFound(typeof(UserGroup).NotFoundMessage(groupMemberId));

        db.Users.Remove(groupMember.User);
        await db.SaveChangesAsync();

        return Result<bool>.SuccessWithData(true);
    }

    public async Task<Result<GroupMemberViewModel>> UpdateGroupMemberAsync(GroupMemberEditModel model)
    {
        if (!await commonService.IsExistAsync<Group>(s => s.Id == model.GroupId))
            return Result<GroupMemberViewModel>.NotFound(typeof(Group).NotFoundMessage(model.GroupId));

        if (!await commonService.IsExistAsync<UserGroupRole>(s => s.Id == model.UserGroupRoleId))
            return Result<GroupMemberViewModel>.NotFound(typeof(UserGroupRole).NotFoundMessage(model.UserGroupRoleId));

        var currentGroupMember = await db.UserGroups.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == model.Id);
        if (currentGroupMember == null)
            return Result<GroupMemberViewModel>.NotFound(typeof(UserGroup).NotFoundMessage(model.Id));

        if (currentGroupMember.GroupId != model.GroupId)
        {
            currentGroupMember.Status = UserGroupStatus.Gona;
            currentGroupMember.PrepareToUpdate(identityService);

            var newGroupMember = new UserGroup
            {
                Title = model.Title,
                Status = UserGroupStatus.Member,
                UserId = currentGroupMember.UserId,
                GroupId = currentGroupMember.GroupId,
                UserGroupRoleId = model.UserGroupRoleId,
            };

            newGroupMember.PrepareToCreate(identityService);
            await db.UserGroups.AddAsync(newGroupMember);
        }
        else
        {
            currentGroupMember.Title = model.Title;
            currentGroupMember.Status = model.Status;
            currentGroupMember.UserGroupRoleId = model.UserGroupRoleId;
            currentGroupMember.PrepareToUpdate(identityService);
        }

        db.UserGroups.Update(currentGroupMember);

        if (model.User != null)
        {
            var user = currentGroupMember.User;
            user.FirstName = model.User.FirstName;
            user.MiddleName = model.User.MiddleName;
            user.LastName = model.User.LastName;
            user.PrepareToUpdate(identityService);
            db.Users.Update(user);
            await sessionService.CloseAllSessionsAsync(currentGroupMember.User.Id);
        }

        await db.SaveChangesAsync();

        return Result<GroupMemberViewModel>.Success();
    }

    private async Task<bool> CanAcceptOrRejectNewJoinersAsync(int groupId)
    {
        if (identityService.IsAdministrator())
            return true;

        var currentUserId = identityService.GetUserId();

        var groupMember = await db.UserGroups.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == currentUserId && s.GroupId == groupId);
        if (groupMember == null)
            return false;
        return groupMember.IsAdmin;
    }
}
