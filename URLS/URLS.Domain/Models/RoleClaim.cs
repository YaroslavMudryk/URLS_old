namespace URLS.Domain.Models;

public class RoleClaim : BaseModel<int>
{
    public int RoleId { get; set; }
    public Role Role { get; set; }

    public int ClaimId { get; set; }
    public Claim Claim { get; set; }
}
