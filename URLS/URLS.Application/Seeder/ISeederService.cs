using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Identity;

namespace URLS.Application.Seeder;

public interface ISeederService
{
    Task<Result<JwtToken>> SeedSystemAsync();
}
