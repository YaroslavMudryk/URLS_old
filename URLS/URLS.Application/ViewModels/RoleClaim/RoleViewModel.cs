namespace URLS.Application.ViewModels.RoleClaim;

public class RoleViewModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; }
    public string Label { get; set; }
    public string Color { get; set; }
    public int CountClaims { get; set; }
    public int[] ClaimIds { get; set; }
    public List<ClaimViewModel> Claims { get; set; }
}
