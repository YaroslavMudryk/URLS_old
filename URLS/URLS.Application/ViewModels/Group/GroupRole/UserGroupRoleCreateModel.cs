using System.ComponentModel.DataAnnotations;
using URLS.Domain.Models;

namespace URLS.Application.ViewModels.Group.GroupRole;

public class UserGroupRoleCreateModel
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; }
    [StringLength(100, MinimumLength = 2)]
    public string NameEng { get; set; }
    [Required, StringLength(10, MinimumLength = 2)]
    public string Color { get; set; }
    [StringLength(250, MinimumLength = 2)]
    public string Description { get; set; }
    [StringLength(250, MinimumLength = 2)]
    public string DescriptionEng { get; set; }
    [Required]
    public UserGroupPermission Permissions { get; set; }
}
