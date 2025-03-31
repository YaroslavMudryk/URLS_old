using System.Security.Claims;

namespace URLS.Application.ViewModels.Identity;

public class UserIdentity
{
    public int Id { get; set; }
    public string Fullname { get; set; }
    public string Login { get; set; }
    public string Username { get; set; }
    public Guid CurrentSessionId { get; set; }
    public IEnumerable<Claim> Claims { get; set; }
    public IEnumerable<string> Roles { get; set; }
    public IEnumerable<int> UserGroupIds { get; set; }

    public bool IsAdministrator
    {
        get
        {
            return Roles == null ? false :
                Roles.Any(x => x.ToLower() == Constants.Roles.Admin.ToLower());
        }
    }
}
