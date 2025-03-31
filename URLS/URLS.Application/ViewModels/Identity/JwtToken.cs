namespace URLS.Application.ViewModels.Identity;

public class JwtToken
{
    public string Token { get; set; }
    public string SessionId { get; set; }
    public DateTime ExpiredAt { get; set; }
}
