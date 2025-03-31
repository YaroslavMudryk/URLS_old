using URLS.Domain.Models;
using System.ComponentModel.DataAnnotations;
using URLS.Application.ViewModels.User;

namespace URLS.Application.ViewModels.Group.GroupMember;

public class GroupMemberEditModel
{
    [Required]
    public int Id { get; set; }
    [StringLength(50)]
    public string Title { get; set; }
    [Required]
    public UserGroupStatus Status { get; set; }
    [Required]
    public int UserGroupRoleId { get; set; }
    public int GroupId { get; set; }
    public UserEditModel User { get; set; }
}
