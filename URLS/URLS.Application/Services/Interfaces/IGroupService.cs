using URLS.Application.Options;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group;
using URLS.Application.ViewModels.Group.GroupMember;

namespace URLS.Application.Services.Interfaces;

public interface IGroupService
{
    Task<Result<List<GroupViewModel>>> GetGroupsBySpecialtyIdAsync(int specialtyId);
    Task<Result<List<GroupViewModel>>> GetAllGroupsAsync(int offset = 0, int limit = 20);
    Task<Result<GroupViewModel>> GetGroupByIdAsync(int id);
    Task<Result<List<GroupViewModel>>> SearchGroupsAsync(SearchGroupOptions options);
    Task<Result<GroupViewModel>> CreateGroupAsync(GroupCreateModel model);
    Task<Result<GroupViewModel>> IncreaseCourseOfGroupAsync(int groupId);
    Task<Result<GroupMemberViewModel>> UpdateClassTeacherGroupAsync(GroupClassTeacherEditModel model);
    Task<Result<List<GroupShortViewModel>>> GetUserGroupsAsync(int userId);
}
