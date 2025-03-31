using Extensions.Password;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Identity;
using URLS.Application.ViewModels.User;
using URLS.Constants;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class UserManager(URLSDbContext db) : IUserManager
{
    public async Task<Result<DateTime>> BlockUserAsync(User user, DateTime? blockUntil = null)
    {
        if (blockUntil == null)
            blockUntil = DateTime.Now.AddHours(1);
        user.LockoutEnd = blockUntil;
        user.AccessFailedCount = 0;
        db.Users.Update(user);
        await db.SaveChangesAsync();
        return Result<DateTime>.SuccessWithData(user.LockoutEnd.Value);
    }

    public async Task<Result<List<UserLogin>>> GetExternalProvidersByUserAsync(int id)
    {
        if (!await db.Users.AsNoTracking().AnyAsync(s => s.Id == id))
        {
            return Result<List<UserLogin>>.NotFound("User not found");
        }
        return Result<List<UserLogin>>.SuccessWithData(await db.UserLogins.Where(s => s.UserId == id).ToListAsync());
    }

    public async Task<Result<User>> GetUserByIdAsync(int id)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        if (user == null)
            return Result<User>.NotFound("User not found");
        return Result<User>.SuccessWithData(user);
    }

    public async Task<Result<User>> GetUserByLoginAsync(string login)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Login == login);
        if (user == null)
            return Result<User>.NotFound("User not found");
        return Result<User>.SuccessWithData(user);
    }

    public async Task<Result<User>> IncrementAccessFailedAndBlockAsync(int id)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
            return Result<User>.NotFound("User not found");
        if (user.AccessFailedCount >= 5)
        {
            var blockUntil = await BlockUserAsync(user, DateTime.Now.AddHours(1));
            user.LockoutEnd = blockUntil.Data;
            return Result<User>.SuccessWithData(user);
        }
        user.AccessFailedCount++;
        db.Users.Update(user);
        await db.SaveChangesAsync();
        return Result<User>.SuccessWithData(user);
    }

    public async Task<Result<(bool, DateTime?)>> IsBlockUserAsync(int id)
    {
        var userResult = await GetUserByIdAsync(id);
        if (!userResult.IsSuccess)
            return Result<(bool, DateTime?)>.NotFound();
        var user = userResult.Data;
        if (user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
        {
            user.LockoutEnd = null;
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return Result<(bool, DateTime?)>.SuccessWithData((false, null));
        }
        return Result<(bool, DateTime?)>.SuccessWithData((true, user.LockoutEnd));
    }

    public async Task<Result<UserLogin>> LinkExternalProviderToCurrentUserAsync(ExternalProviderInfo externalProvider, int userId)
    {
        if (!await db.Users.AsNoTracking().AnyAsync(s => s.Id == userId))
            return Result<UserLogin>.NotFound("User not found");
        var userLogin = new UserLogin
        {
            ExternalProvider = externalProvider.Provider,
            Email = externalProvider.Email,
            Key = externalProvider.Key,
            UserId = userId
        };
        await db.UserLogins.AddAsync(userLogin);
        await db.SaveChangesAsync();
        return Result<UserLogin>.SuccessWithData(userLogin);
    }

    public async Task<Result<User>> LoginAsync(string login, string password)
    {
        var user = await db.Users.FirstOrDefaultAsync(s => s.Login == login);
        if (user == null)
            return Result<User>.NotFound("User by login not found");
        if (user.IsLocked())
            return Result<User>.Error($"User is banned [{user.LockoutEnd}]");
        if (!user.PasswordHash.VerifyPasswordHash(password))
            return Result<User>.Error("Password is incorrect");
        return Result<User>.SuccessWithData(user);
    }

    public async Task<Result<User>> LoginByExternalProviderAsync(ExternalProviderInfo externalProvider)
    {
        var userLogin = await db.UserLogins
            .FirstOrDefaultAsync(s =>
            s.ExternalProvider == externalProvider.Provider &&
            s.Key == externalProvider.Key &&
            s.Email == externalProvider.Email);
        if (userLogin == null)
            return Result<User>.NotFound("User by external creds not found");
        var userResult = await GetUserByIdAsync(userLogin.UserId);
        if (userResult.IsSuccess)
        {
            var user = userResult.Data;
            if (user.IsLocked())
                return Result<User>.Error("User is banned");
        }
        return Result<User>.SuccessWithData(userResult.Data);
    }

    public async Task<Result<User>> RegisterAsync(RegisterViewModel registerModel)
    {
        if (await db.Users.AsNoTracking().AnyAsync(s => s.Login == registerModel.Login))
            return Result<User>.Error("Login is busy");
        var newUser = new User(registerModel.FirstName, null, registerModel.LastName, registerModel.Login, Generator.GetString(25));
        newUser.PasswordHash = registerModel.Password.GeneratePasswordHash();
        await db.Users.AddAsync(newUser);
        await db.SaveChangesAsync();
        return Result<User>.SuccessWithData(newUser);
    }

    public async Task<Result<User>> UnBlockUserAsync(int id)
    {
        var userResult = await GetUserByIdAsync(id);
        if (userResult.IsNotFound)
            return userResult;
        var user = userResult.Data;
        user.LockoutEnd = null;
        user.AccessFailedCount = 0;
        db.Users.Update(user);
        await db.SaveChangesAsync();
        return Result<User>.SuccessWithData(user);
    }
}
