using URLS.Application.ViewModels.Identity;

namespace URLS.Application.Services.Interfaces;

public interface ITokenService
{
    Task<JwtToken> GetUserTokenAsync(UserTokenModel userToken);
}
