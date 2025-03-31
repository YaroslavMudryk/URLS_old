using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.RoleClaim;

public class ClaimEditModel
{
    public int Id { get; set; }
    [StringLength(500, MinimumLength = 1)]
    public string DisplayName { get; set; }
}
