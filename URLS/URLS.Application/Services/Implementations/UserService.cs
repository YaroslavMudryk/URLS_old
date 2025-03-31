using AutoMapper;
using Extensions.Password;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Helpers;
using URLS.Application.Options;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group.GroupMember;
using URLS.Application.ViewModels.RoleClaim;
using URLS.Application.ViewModels.Session;
using URLS.Application.ViewModels.User;
using URLS.Application.ViewModels.User.UserInfo;
using URLS.Constants;
using URLS.Constants.APIResponse;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class UserService(
    URLSDbContext db,
    IMapper mapper,
    IIdentityService identityService,
    ICommonService commonService) : IUserService
{
    public async Task<Result<UserViewModel>> CreateUserAsync(UserCreateModel model)
    {
        if (await commonService.IsExistAsync<User>(s => s.Login == model.Login))
            return Result<UserViewModel>.Error("Login is busy");

        if (!await db.Roles.AsNoTracking().AnyAsync(s => s.Id == model.RoleId))
            return Result<UserViewModel>.NotFound("Role not found");

        if (!string.IsNullOrEmpty(model.UserName))
            if (await commonService.IsExistAsync<User>(s => s.UserName == model.UserName))
                return Result<UserViewModel>.Error("Username is busy");

        var newUser = new User(model.FirstName, model.MiddleName, model.LastName, model.Login, null);
        newUser.UserName = model.UserName ?? Generator.GetUsername();
        newUser.PasswordHash = model.Password.GeneratePasswordHash();
        newUser.NotificationSettings = new NotificationSettings
        {
            AcceptedInGroup = true,
            ChangePassword = true,
            Logout = true,
            NewLogin = true,
            NewPost = true,
            Welcome = true
        };
        newUser.IsActivateAccount = true;
        newUser.FromImport = false;
        newUser.ModifiedFromTemp = null;
        newUser.PrepareToCreate(identityService);
        await db.Users.AddAsync(newUser);
        await db.SaveChangesAsync();

        var userRole = new UserRole
        {
            UserId = newUser.Id,
            RoleId = model.RoleId
        };
        userRole.PrepareToCreate(identityService);

        await db.UserRoles.AddAsync(userRole);

        var notify = NotificationsHelper.GetWelcomeNotification();
        notify.UserId = newUser.Id;

        await db.Notifications.AddAsync(notify);
        await db.SaveChangesAsync();

        return Result<UserViewModel>.Created(mapper.Map<UserViewModel>(newUser)); ;
    }

    public async Task<Result<UserFullViewModel>> GetFullInfoUserByIdAsync(int id)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
            return Result<UserFullViewModel>.NotFound("User not found");
        var userToView = mapper.Map<UserFullViewModel>(user);

        userToView.Block = new BlockInfo
        {
            AccessFailedCount = user.AccessFailedCount,
            LockoutEnabled = user.LockoutEnabled,
            LockoutEnd = user.LockoutEnd
        };

        var sessions = await db.Sessions.AsNoTracking().Where(x => x.UserId == id).ToListAsync();

        if (sessions != null && sessions.Count > 0)
            userToView.Session = new SessionInfo
            {
                Sessions = mapper.Map<List<SessionViewModel>>(sessions),
                TotalSessions = sessions.Count,
                ActiveSessions = sessions.Count(s => s.IsActive)
            };

        var userRoles = await db.UserRoles.AsNoTracking().Include(s => s.Role).Where(s => s.UserId == id).Select(s => s.Role).ToListAsync();
        if (userRoles != null && userRoles.Count > 0)
            userToView.Role = new RoleInfo
            {
                Roles = mapper.Map<List<RoleViewModel>>(userRoles)
            };

        var groupMembers = await db.UserGroups.AsNoTracking().Include(s => s.Group).Include(s => s.UserGroupRole).Where(s => s.UserId == id).ToListAsync();
        if (groupMembers != null && groupMembers.Count > 0)
            userToView.Group = new GroupInfo
            {
                Groups = mapper.Map<List<GroupMemberViewModel>>(groupMembers).OrderByDescending(s=>s.Group.Name).ToList()
            };

        return Result<UserFullViewModel>.SuccessWithData(userToView);
    }

    public async Task<Result<List<UserShortViewModel>>> GetLastUsersAsync(int count)
    {
        var lastUsers = await db.Users
            .AsNoTracking()
            .OrderByDescending(s => s.JoinAt)
            .Take(count)
            .ToListAsync();
        return Result<List<UserShortViewModel>>.SuccessWithData(mapper.Map<List<UserShortViewModel>>(lastUsers));
    }

    public async Task<Result<List<UserShortViewModel>>> GetTeachersAsync(int offset = 0, int count = 20)
    {
        var teachers = await db.UserRoles
            .Where(s => s.RoleId == 4)
            .Include(s => s.User)
            .OrderBy(s => s.UserId)
            .Skip(offset).Take(count)
            .Select(s => new UserShortViewModel
            {
                Id = s.User.Id,
                FirstName = s.User.FirstName,
                LastName = s.User.LastName,
                UserName = s.User.UserName,
                Image = s.User.Image,
                JoinAt = s.User.JoinAt
            })
            .ToListAsync();

        var totalCount = await db.UserRoles.CountAsync(s => s.RoleId == 4);

        return Result<List<UserShortViewModel>>.SuccessList(teachers, Meta.FromMeta(totalCount, offset, count));
    }

    public async Task<Result<UserViewModel>> GetUserByIdAsync(int id)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
            return Result<UserViewModel>.NotFound("User not found");
        return Result<UserViewModel>.SuccessWithData(mapper.Map<UserViewModel>(user));
    }

    public async Task<Result<List<UserShortViewModel>>> SearchUsersAsync(SearchUserOptions searchUserOptions)
    {
        searchUserOptions.PrepareOptions();

        IQueryable<User> query = db.Users;

        query = query.AsNoTracking();

        query = query.Skip(searchUserOptions.Offset).Take(searchUserOptions.Count);

        if (!string.IsNullOrEmpty(searchUserOptions.FirstName))
            query = query.Where(x => x.FirstName.Contains(searchUserOptions.FirstName));

        if (!string.IsNullOrEmpty(searchUserOptions.LastName))
            query = query.Where(x => x.LastName.Contains(searchUserOptions.LastName));

        //Other filters


        var result = await query.OrderBy(x => x.Id).ToListAsync();

        return Result<List<UserShortViewModel>>.SuccessWithData(mapper.Map<List<UserShortViewModel>>(result));
    }

    public async Task<Result<NotificationSettings>> UpdateNotificationSettingsAsync(int userId, NotificationSettings notificationSettings)
    {
        if (!identityService.IsAdministrator())
            if (userId != identityService.GetUserId())
                return Result<NotificationSettings>.Error("Access denited");

        var userToUpdate = await db.Users.FindAsync(userId);

        userToUpdate.NotificationSettings = notificationSettings;

        userToUpdate.PrepareToUpdate(identityService);

        db.Users.Update(userToUpdate);
        await db.SaveChangesAsync();

        return Result<NotificationSettings>.SuccessWithData(notificationSettings);
    }

    public async Task<Result<UserViewModel>> UpdateUsernameAsync(UsernameUpdateModel model)
    {
        if (model.UserId != identityService.GetUserId())
            if (!identityService.GetRoles().Contains(Roles.Admin))
                return Result<UserViewModel>.Error("Access denited");

        var userToUpdate = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.UserId);
        if (userToUpdate == null)
            return Result<UserViewModel>.NotFound("User by id not found");

        if (userToUpdate.UserName == model.Username)
            return Result<UserViewModel>.Error("Username equals current you");

        if (await commonService.IsExistAsync<User>(s => s.UserName == model.Username))
            return Result<UserViewModel>.Error("Username is already busy");

        userToUpdate.UserName = model.Username;
        userToUpdate.PrepareToUpdate(identityService);
        db.Users.Update(userToUpdate);
        await db.SaveChangesAsync();
        return Result<UserViewModel>.SuccessWithData(mapper.Map<UserViewModel>(userToUpdate));
    }
}
