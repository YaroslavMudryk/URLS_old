namespace URLS.Application.ViewModels.Identity;

public class SocialViewModel
{
    public int Id { get; set; }
    public string Provider { get; set; }
    public DateTime LinkedAt { get; set; }
    public DateTime? LastSigIn { get; set; }
}
