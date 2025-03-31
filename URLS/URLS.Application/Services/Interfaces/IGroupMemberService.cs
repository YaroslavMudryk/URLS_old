using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group.GroupMember;
using URLS.Application.ViewModels.User;

namespace URLS.Application.Services.Interfaces;

public interface IGroupMemberService
{
    Task<Result<List<GroupMemberViewModel>>> GetGroupMembersAsync(int groupId, int offset = 0, int count = 20, int status = 0);
    Task<Result<GroupMemberViewModel>> GetGroupMemberByIdAsync(int groupId, int memberId);
    Task<Result<GroupMemberViewModel>> UpdateGroupMemberAsync(GroupMemberEditModel model);
    Task<Result<bool>> AcceptAllNewGroupMembersAsync(int groupId);
    Task<Result<bool>> AcceptNewGroupMemberAsync(int groupId, int groupMemberId, UserEditModel editModel);
    Task<Result<bool>> RejectNewGroupMemberAsync(int groupId, int groupMemberId);
}
