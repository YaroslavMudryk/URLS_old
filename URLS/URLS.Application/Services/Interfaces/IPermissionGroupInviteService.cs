namespace URLS.Application.Services.Interfaces;

public interface IPermissionGroupInviteService
{
    Task<bool> CanCreateInviteAsync(int groupId);
    Task<bool> CanViewInviteAsync(int groupId);
    Task<bool> CanRemoveInviteAsync(int groupId);
    Task<bool> CanUpdateInviteAsync(int groupId);
}
