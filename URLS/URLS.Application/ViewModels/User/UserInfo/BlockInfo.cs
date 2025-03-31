namespace URLS.Application.ViewModels.User.UserInfo;

public class BlockInfo
{
    public int AccessFailedCount { get; set; }
    public bool LockoutEnabled { get; set; }
    public DateTime? LockoutEnd { get; set; }
}
