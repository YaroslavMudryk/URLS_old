using URLS.Constants;
using URLS.Domain.Models;

namespace URLS.Application.Extensions;

public static class UserExtensions
{
    public static void ModifyUserFromImport(this User user)
    {
        user.Login = user.CreateTempLogin();
        user.PasswordHash = Generator.CreateTempPassword();
        user.FromImport = true;
        user.ModifiedFromTemp = null;
        user.PrepareToCreate();
    }

    public static string CreateTempLogin(this User user)
    {
        return $"{user.UserName}@email.com";
    }
}
