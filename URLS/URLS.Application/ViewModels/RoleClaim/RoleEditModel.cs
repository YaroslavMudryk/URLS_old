using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.RoleClaim;

public class RoleEditModel : RoleCreateModel
{
    [Required]
    public int Id { get; set; }
}
