namespace URLS.Application.ViewModels.User.UserInfo;

public class UserFullViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public DateTime JoinAt { get; set; }
    public string Image { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhone { get; set; }
    public bool MFA { get; set; }
    public RoleInfo Role { get; set; }
    public SessionInfo Session { get; set; }
    public GroupInfo Group { get; set; }
    public BlockInfo Block { get; set; }
}
