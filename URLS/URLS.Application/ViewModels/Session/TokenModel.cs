namespace URLS.Application.ViewModels.Session;

public class TokenModel
{
    public string Token { get; set; }
    public DateTime ExpiredAt { get; set; }

    public TokenModel()
    {

    }

    public TokenModel(string token, DateTime expiredAt)
    {
        Token = token;
        ExpiredAt = expiredAt;
    }
}
