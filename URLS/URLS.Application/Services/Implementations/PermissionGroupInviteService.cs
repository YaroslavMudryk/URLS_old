using Microsoft.EntityFrameworkCore;
using URLS.Application.Services.Interfaces;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class PermissionGroupInviteService(
    URLSDbContext db,
    IIdentityService identityService) : IPermissionGroupInviteService
{
    public async Task<bool> CanCreateInviteAsync(int groupId)
    {
        if (identityService.IsAdministrator())
            return true;
        var member = await db.UserGroups.AsNoTracking().Include(s => s.UserGroupRole).FirstOrDefaultAsync(s => s.GroupId == groupId && s.UserId == identityService.GetUserId() && s.Status == UserGroupStatus.Member);
        if (member == null)
            return false;
        if (!member.UserGroupRole.Permissions.CanCreateInviteCode)
            return false;
        return true;
    }

    public async Task<bool> CanRemoveInviteAsync(int groupId)
    {
        if (identityService.IsAdministrator())
            return true;
        var member = await db.UserGroups.AsNoTracking().Include(s => s.UserGroupRole).FirstOrDefaultAsync(s => s.GroupId == groupId && s.UserId == identityService.GetUserId() && s.Status == UserGroupStatus.Member);
        if (member == null)
            return false;
        if (!member.UserGroupRole.Permissions.CanRemoveInviteCode)
            return false;
        return true;
    }

    public async Task<bool> CanUpdateInviteAsync(int groupId)
    {
        if (identityService.IsAdministrator())
            return true;
        var member = await db.UserGroups.AsNoTracking().Include(s => s.UserGroupRole).FirstOrDefaultAsync(s => s.GroupId == groupId && s.UserId == identityService.GetUserId() && s.Status == UserGroupStatus.Member);
        if (member == null)
            return false;
        if (!member.UserGroupRole.Permissions.CanUpdateInviteCode)
            return false;
        return true;
    }

    public async Task<bool> CanViewInviteAsync(int groupId)
    {
        if (identityService.IsAdministrator())
            return true;
        var member = await db.UserGroups.AsNoTracking().Include(s => s.UserGroupRole).FirstOrDefaultAsync(s => s.GroupId == groupId && s.UserId == identityService.GetUserId() && s.Status == UserGroupStatus.Member);
        if (member == null)
            return false;
        if (!member.UserGroupRole.Permissions.CanViewInviteCodes)
            return false;
        return true;
    }
}
