namespace URLS.Application.ViewModels.Identity;

public class UserTokenModel
{
    public int UserId { get; set; }
    public Domain.Models.User User { get; set; }
    public Guid SessionId { get; set; }
    public string AuthType { get; set; }
    public string Lang { get; set; }
}
