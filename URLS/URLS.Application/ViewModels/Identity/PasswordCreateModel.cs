namespace URLS.Application.ViewModels.Identity;

public class PasswordCreateModel
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public bool LogoutEverywhere { get; set; }
}
