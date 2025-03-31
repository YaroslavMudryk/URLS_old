using System.ComponentModel.DataAnnotations;
namespace URLS.Domain.Models;

public class Role : BaseModel<int>
{
    [Required, StringLength(150, MinimumLength = 1)]
    public string Name { get; set; }
    [StringLength(150, MinimumLength = 3)]
    public string Label { get; set; }
    [StringLength(10, MinimumLength = 1)]
    public string Color { get; set; }
    public List<RoleClaim> RoleClaims { get; set; }
    public int? CountClaims { get; set; }
    public string ClaimsHash { get; set; }
    public bool CanDelete { get; set; }

    public Role(string name)
    {
        Name = name;
    }
    public Role()
    {

    }
}
