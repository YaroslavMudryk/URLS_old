using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Identity;

public class UserRoleSetupModel
{
    public int UserId { get; set; }
    [Required]
    public int[] RoleIds { get; set; }
    public string Password { get; set; }
}
