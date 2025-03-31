using Microsoft.EntityFrameworkCore;
using URLS.Application.Services.Interfaces;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class PermissionCommentService(
    URLSDbContext db,
    IIdentityService identityService) : IPermissionCommentService
{
    public async Task<bool> CanCreateCommentAsync(int groupId)
    {
        if (identityService.IsAdministrator())
            return true;

        var member = await db.UserGroups
            .AsNoTracking()
            .Include(s => s.UserGroupRole)
            .FirstOrDefaultAsync(s => s.Status == Domain.Models.UserGroupStatus.Member && s.GroupId == groupId && s.UserId == identityService.GetUserId());

        if (member == null)
        {
            var subjects = await db.Subjects.AsNoTracking().Where(s => s.TeacherId == identityService.GetUserId() && s.GroupId == groupId).ToListAsync();
            return subjects == null || subjects.Count == 0 ? false : true;
        }
        else
        {
            return member.UserGroupRole.Permissions.CanCreateComment;
        }
    }

    public async Task<bool> CanViewAllCommentsAsync(int groupId, int postId)
    {
        if (identityService.IsAdministrator())
            return true;

        var member = await db.UserGroups
            .AsNoTracking()
            .Include(s => s.UserGroupRole)
            .FirstOrDefaultAsync(s => s.Status == Domain.Models.UserGroupStatus.Member && s.GroupId == groupId && s.UserId == identityService.GetUserId());

        if (member == null)
        {
            var subjects = await db.Subjects.AsNoTracking().Where(s => s.TeacherId == identityService.GetUserId() && s.GroupId == groupId).ToListAsync();
            return subjects == null || subjects.Count == 0 ? false : true;
        }
        else
        {
            return member.UserGroupRole.Permissions.CanCreateComment;
        }
    }
}
