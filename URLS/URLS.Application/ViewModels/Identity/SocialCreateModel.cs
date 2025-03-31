using Extensions.DeviceDetector.Models;
namespace URLS.Application.ViewModels.Identity;

public class SocialCreateModel
{
    public string Scheme { get; set; }
    public string Email { get; set; }
    public string UniqId { get; set; }
    public ClientInfo Client { get; set; }
}
