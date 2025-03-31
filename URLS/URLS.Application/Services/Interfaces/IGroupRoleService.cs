using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group.GroupRole;

namespace URLS.Application.Services.Interfaces;

public interface IGroupRoleService
{
    Task<Result<List<UserGroupRoleViewModel>>> GetAllGroupRolesAsync();
    Task<Result<UserGroupRoleViewModel>> GetGroupRoleByIdAsync(int id);
    Task<Result<UserGroupRoleViewModel>> CreateGroupRoleAsync(UserGroupRoleCreateModel model);
    Task<Result<UserGroupRoleViewModel>> UpdateGroupRoleAsync(UserGroupRoleEditModel model);
    Task<Result<bool>> RemoveGroupRoleAsync(int id);
}
