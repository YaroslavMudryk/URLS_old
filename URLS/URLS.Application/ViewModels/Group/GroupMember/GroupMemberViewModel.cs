using URLS.Application.ViewModels.Group.GroupRole;
using URLS.Application.ViewModels.User;
using URLS.Domain.Models;

namespace URLS.Application.ViewModels.Group.GroupMember;

public class GroupMemberViewModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsAdmin { get; set; }
    public string Title { get; set; }
    public UserGroupStatus Status { get; set; }
    public UserGroupRoleViewModel UserGroupRole { get; set; }
    public UserViewModel User { get; set; }
    public GroupViewModel Group { get; set; }
}
