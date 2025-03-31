using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Identity;
using URLS.Constants;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class TokenService(URLSDbContext db) : ITokenService
{
    public async Task<JwtToken> GetUserTokenAsync(UserTokenModel userToken)
    {
        userToken.Lang = userToken.Lang.ToLower();

        var user = userToken.User ?? await db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == userToken.UserId);

        if (user == null)
            return null;

        var currentUserRoles = await db.UserRoles
            .Where(x => x.UserId == user.Id)
            .Include(x => x.Role)
            .Select(x => x.Role)
            .ToListAsync();

        var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(CustomClaimTypes.CurrentSessionId, userToken.SessionId.ToString()),
            new System.Security.Claims.Claim(CustomClaimTypes.Login, user.Login),
            new System.Security.Claims.Claim(CustomClaimTypes.UserId, user.Id.ToString()),
            new System.Security.Claims.Claim(CustomClaimTypes.UserName, user.UserName),
            new System.Security.Claims.Claim(CustomClaimTypes.FullName, $"{user.LastName} {user.FirstName}"),
            new System.Security.Claims.Claim(CustomClaimTypes.AuthenticationMethod, userToken.AuthType),
            new System.Security.Claims.Claim(CustomClaimTypes.Language, userToken.Lang)
        };

        foreach (var role in currentUserRoles)
        {
            claims.Add(new System.Security.Claims.Claim(CustomClaimTypes.Role, role.Name));
        }

        var userGroups = await db.UserGroups.AsNoTracking().Where(x => x.UserId == user.Id && x.Status == UserGroupStatus.Member).ToListAsync();

        if (userGroups != null && userGroups.Any())
        {
            foreach (var userGroup in userGroups)
            {
                claims.Add(new System.Security.Claims.Claim(CustomClaimTypes.GroupMemberId, userGroup.Id.ToString()));
            }
        }


        var roleIds = currentUserRoles.Select(x => x.Id);

        var roleClaims = await db.RoleClaims.Where(s => roleIds.Contains(s.RoleId)).Include(s => s.Claim).ToListAsync();

        var userRoleClaims = GetUniqClaims(roleClaims);

        if (userRoleClaims != null && userRoleClaims.Count > 0)
        {
            foreach (var roleClaim in userRoleClaims)
            {
                claims.Add(new System.Security.Claims.Claim(roleClaim.Type, roleClaim.Value));
            }
        }

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        var now = DateTime.Now;
        var expiredAt = now.Add(TimeSpan.FromDays(TokenOptions.LifeTimeInDays));
        var jwt = new JwtSecurityToken(
                issuer: TokenOptions.Issuer,
                audience: TokenOptions.Audience,
                notBefore: now,
                claims: claimsIdentity.Claims,
                expires: expiredAt,
                signingCredentials: new SigningCredentials(TokenOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
        return new JwtToken
        {
            Token = encodedJwt,
            ExpiredAt = expiredAt,
            SessionId = userToken.SessionId.ToString()
        };
    }

    private List<Domain.Models.Claim> GetUniqClaims(List<RoleClaim> roleClaims)
    {
        return roleClaims.Select(s => new { Type = s.Claim.Type, Value = s.Claim.Value }).Distinct().Select(x => new Domain.Models.Claim { Type = x.Type, Value = x.Value }).ToList();
    }
}
