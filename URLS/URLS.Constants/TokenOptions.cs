using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace URLS.Constants;

public class TokenOptions
{
    public const string Issuer = "URLS ID";
    public const string Audience = "URLS Client";
    const string SecretKey = "418b66dc05744f90b3fdc246ecc7dd372ca30e632f1744649f1df0cef16e2222"; // temp
    public const int LifeTimeInDays = 45;
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
}
