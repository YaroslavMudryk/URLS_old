using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Group.GroupRole;

public class UserGroupRoleEditModel : UserGroupRoleCreateModel
{
    [Required]
    public int Id { get; set; }
}
