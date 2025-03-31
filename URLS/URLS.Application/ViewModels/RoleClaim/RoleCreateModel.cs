using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.RoleClaim;

public class RoleCreateModel
{
    [Required, StringLength(150, MinimumLength = 1)]
    public string Name { get; set; }
    [StringLength(150, MinimumLength = 3)]
    public string Label { get; set; }
    [StringLength(10, MinimumLength = 1)]
    public string Color { get; set; }
    public int[] ClaimsIds { get; set; }
}
