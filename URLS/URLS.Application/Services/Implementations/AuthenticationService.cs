using AutoMapper;
using Extensions.DeviceDetector;
using Extensions.Password;
using Google.Authenticator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Helpers;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Identity;
using URLS.Application.ViewModels.RoleClaim;
using URLS.Application.ViewModels.Session;
using URLS.Application.ViewModels.User;
using URLS.Constants;
using URLS.Constants.Extensions;
using URLS.Constants.Localisation;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class AuthenticationService(
    URLSDbContext db,
    IIdentityService identityService,
    ISessionManager sessionManager,
    ILocationService locationService,
    ITokenService tokenService,
    IDetector detector,
    IMapper mapper,
    ICommonService commonService,
    IAppService appService,
    ILocalizeService localizeService,
    ISessionService sessionService) : Interfaces.IAuthenticationService
{
    public async Task<Result<UserViewModel>> BlockUserConfigAsync(BlockUserModel model)
    {
        if (!identityService.IsAdministrator())
            return Result<UserViewModel>.Forbiden();

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == model.UserId);
        if (user == null)
            return Result<UserViewModel>.NotFound(typeof(User).NotFoundMessage(model.UserId));
        var count = model.AccessFailedCount;
        if (count < 0 || count > 5)
            model.AccessFailedCount = 0;
        else
            user.AccessFailedCount = model.AccessFailedCount;

        user.LockoutEnabled = model.LockoutEnabled;
        user.LockoutEnd = model.LockoutEnd;

        user.PrepareToUpdate(identityService);
        db.Users.Update(user);
        await db.SaveChangesAsync();

        return Result<UserViewModel>.SuccessWithData(mapper.Map<UserViewModel>(user));
    }

    public async Task<Result<AuthenticationInfo>> ChangePasswordAsync(PasswordCreateModel model)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == identityService.GetUserId());
        if (user == null)
            return Result<AuthenticationInfo>.NotFound(typeof(User).NotFoundMessage(identityService.GetUserId()));

        if (!model.OldPassword.VerifyPasswordHash(user.PasswordHash))
            return Result<AuthenticationInfo>.Error(localizeService.Get("passwordnotcomparer"));

        if (model.OldPassword == model.NewPassword)
            return Result<AuthenticationInfo>.Error("This passwords are match");

        user.PasswordHash = model.NewPassword.GeneratePasswordHash();

        user.PrepareToUpdate(identityService);

        db.Users.Update(user);
        await db.SaveChangesAsync();
        var notification = NotificationsHelper.GetChangePasswordNotification();
        notification.UserId = user.Id;
        await db.Notifications.AddAsync(notification);
        await db.SaveChangesAsync();

        if (model.LogoutEverywhere)
        {
            var res = await sessionService.CloseAllSessionsAsync(identityService.GetUserId());
            if (!res.IsSuccess)
                return res.MapToNew<AuthenticationInfo>(null);
        }

        return Result<AuthenticationInfo>.SuccessWithData(new AuthenticationInfo
        {
            User = mapper.Map<UserViewModel>(user),
        });
    }

    public async Task<Result<bool>> DisableMFAAsync(string code)
    {
        var userId = identityService.GetUserId();

        var userForDisableMFA = await db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == userId);
        if (userForDisableMFA == null)
            return Result<bool>.NotFound(typeof(User).NotFoundMessage(userId));

        if (!userForDisableMFA.MFA)
            return Result<bool>.Error("MFA already diactivated");

        var twoFactor = new TwoFactorAuthenticator();

        if (!twoFactor.ValidateTwoFactorPIN(userForDisableMFA.MFASecretKey, code))
            return Result<bool>.Error("Code is incorrect");

        userForDisableMFA.MFA = false;
        userForDisableMFA.MFASecretKey = null;
        userForDisableMFA.PrepareToUpdate(identityService);
        db.Users.Update(userForDisableMFA);
        await db.SaveChangesAsync();

        var activeMFA = await db.MFAs.FirstOrDefaultAsync(s => s.UserId == userId && s.IsActivated);
        if (activeMFA == null)
            return Result<bool>.Error("Some error, please contact support");

        activeMFA.Diactived = DateTime.Now;
        activeMFA.DiactivedBySessionId = identityService.GetCurrentSessionId();
        activeMFA.PrepareToUpdate(identityService);

        db.MFAs.UpdateRange(activeMFA);
        await db.SaveChangesAsync();

        return Result<bool>.Success();
    }

    public async Task<Result<MFAViewModel>> EnableMFAAsync(string code = null)
    {
        var userId = identityService.GetUserId();
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == userId);

        if (user == null)
            return Result<MFAViewModel>.NotFound(typeof(User).NotFoundMessage(userId));

        if (code == null)
        {
            var existMFA = await db.MFAs.FirstOrDefaultAsync(s => s.UserId == userId && !s.IsActivated);
            if (existMFA == null)
            {
                var secretKey = Guid.NewGuid().ToString("N");
                var twoFactor = new TwoFactorAuthenticator();
                var setupInfo = twoFactor.GenerateSetupCode("URLS", user.Login, secretKey, false, 3);

                user.MFASecretKey = secretKey;
                user.MFA = false;
                user.PrepareToUpdate(identityService);
                db.Users.Update(user);
                await db.SaveChangesAsync();

                var newMFA = new MFA
                {
                    UserId = userId,
                    EntryCode = setupInfo.ManualEntryKey,
                    QrCodeBase64 = setupInfo.QrCodeSetupImageUrl,
                    Secret = secretKey,
                    IsActivated = false,
                    Activated = null,
                    ActivatedBySessionId = null
                };

                newMFA.PrepareToCreate(identityService);
                await db.MFAs.AddAsync(newMFA);
                await db.SaveChangesAsync();
                return Result<MFAViewModel>.SuccessWithData(new MFAViewModel
                {
                    QrCodeImage = setupInfo.QrCodeSetupImageUrl,
                    ManualEntryKey = setupInfo.ManualEntryKey
                });
            }
            else
            {
                return Result<MFAViewModel>.SuccessWithData(new MFAViewModel
                {
                    QrCodeImage = existMFA.QrCodeBase64,
                    ManualEntryKey = existMFA.EntryCode
                });
            }
        }
        else
        {
            if (string.IsNullOrEmpty(user.MFASecretKey))
                return Result<MFAViewModel>.Error("Unable to activate MFA");

            var mfaToActivate = await db.MFAs.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == userId && !s.IsActivated);

            if (mfaToActivate == null)
                return Result<MFAViewModel>.Error("Unable to activate MFA");

            if (mfaToActivate.Secret != user.MFASecretKey)
                return Result<MFAViewModel>.Error("Please write to support as soon as possible");

            var twoFactor = new TwoFactorAuthenticator();

            if (!twoFactor.ValidateTwoFactorPIN(mfaToActivate.Secret, code))
                return Result<MFAViewModel>.Error("Code is incorrect");

            user.MFA = true;
            user.PrepareToUpdate(identityService);
            db.Users.Update(user);
            await db.SaveChangesAsync();

            mfaToActivate.IsActivated = true;
            mfaToActivate.Activated = DateTime.Now;
            mfaToActivate.ActivatedBySessionId = identityService.GetCurrentSessionId();

            mfaToActivate.PrepareToUpdate(identityService);

            db.MFAs.Update(mfaToActivate);
            await db.SaveChangesAsync();

            return Result<MFAViewModel>.Success();
        }
    }

    public async Task<Result<List<SocialViewModel>>> GetUserLoginsAsync(int userId)
    {
        var userLogins = await db.UserLogins.AsNoTracking().Where(s => s.UserId == userId).ToListAsync();

        var socials = mapper.Map<List<SocialViewModel>>(userLogins);

        foreach (var social in socials)
        {
            var lastSession = await db.Sessions.AsNoTracking().OrderByDescending(s => s.CreatedBy).FirstOrDefaultAsync(s => s.Type == social.Provider);
            if (lastSession != null)
                social.LastSigIn = lastSession.CreatedAt;
        }
        return Result<List<SocialViewModel>>.SuccessWithData(socials);
    }

    public async Task<Result<List<RoleViewModel>>> GetUserRolesAsync(int userId)
    {
        var roles = await db.UserRoles
            .AsNoTracking()
            .Include(s => s.Role)
            .Where(s => s.UserId == userId)
            .Select(s => s.Role)
            .ToListAsync();

        var rolesToView = mapper.Map<List<RoleViewModel>>(roles);
        return Result<List<RoleViewModel>>.SuccessWithData(rolesToView);
    }

    public async Task<Result<bool>> LinkSocialAsync(SocialCreateModel model)
    {
        var loginResult = await commonService.IsExistWithResultsAsync<UserLogin>(s =>
            s.ExternalProvider == model.Scheme &&
            s.Email == model.Email &&
            s.Key == model.UniqId);

        if (loginResult.IsExist)
        {
            if (loginResult.Results.First().UserId == identityService.GetUserId())
                return Result<bool>.Success();
            return Result<bool>.Error("This account is linked to another account");
        }
        else
        {
            var newUserLogin = new UserLogin
            {
                ExternalProvider = model.Scheme,
                Email = model.Email,
                Key = model.UniqId,
                UserId = identityService.GetUserId()
            };
            newUserLogin.PrepareToCreate(identityService);
            await db.UserLogins.AddAsync(newUserLogin);
            await db.SaveChangesAsync();
            return Result<bool>.Success();
        }
    }

    public async Task<Result<JwtToken>> LoginByMFAAsync(LoginMFAModel model)
    {
        var sessionId = Guid.Parse(model.SessionId);

        var session = await db.Sessions.AsNoTracking().Include(s => s.User).FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
            return Result<JwtToken>.NotFound(typeof(Session).NotFoundMessage(model.SessionId));

        var secretKey = session.User.MFASecretKey;

        var twoFactor = new TwoFactorAuthenticator();

        var result = twoFactor.ValidateTwoFactorPIN(secretKey, model.Code);
        if (!result)
            return Result<JwtToken>.Error("Code is incorrect");

        return Result<JwtToken>.SuccessWithData(new JwtToken
        {
            Token = session.Token,
            SessionId = model.SessionId,
            ExpiredAt = session.ExpiredAt
        });
    }

    public async Task<Result<JwtToken>> LoginByPasswordAsync(LoginCreateModel model)
    {
        var app = await db.Apps
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AppId == model.App.Id && x.AppSecret == model.App.Secret);

        if (app == null)
            return Result<JwtToken>.NotFound(typeof(App).NotFoundMessage(model.App.Id));

        if (!app.IsActive)
            return Result<JwtToken>.Error("App already unactive");

        if (!app.IsActiveByTime())
            return Result<JwtToken>.Error("App is expired");

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Login == model.Login);
        if (user == null)
            return Result<JwtToken>.NotFound(typeof(User).NotFoundMessage(model.Login));

        if (!user.IsActivateAccount)
            return Result<JwtToken>.Error("Account not activated");

        if (user.LockoutEnabled)
        {
            if (user.IsLocked())
            {
                return Result<JwtToken>.Error($"Your account has been locked up to {user.LockoutEnd.Value.ToString("HH:mm (dd.MM.yyyy)")}");
            }

            if (user.AccessFailedCount == 5)
            {
                user.AccessFailedCount = 0;
                user.LockoutEnd = DateTime.Now.AddHours(1);
                var notifyLogin = NotificationsHelper.GetLockedNotification();
                notifyLogin.UserId = user.Id;
                db.Users.Update(user);
                await db.Notifications.AddAsync(notifyLogin);
                await db.SaveChangesAsync();

                return Result<JwtToken>.Error($"Account locked up to {user.LockoutEnd.Value.ToString("HH:mm (dd.MM.yyyy)")}");
            }
        }

        if (!model.Password.VerifyPasswordHash(user.PasswordHash))
        {
            user.AccessFailedCount++;
            db.Users.Update(user);
            var loginAttemptNotify = NotificationsHelper.GetLoginAttemptNotification(model, identityService.GetIP());
            loginAttemptNotify.UserId = user.Id;
            await db.Notifications.AddAsync(loginAttemptNotify);
            await db.SaveChangesAsync();
            return Result<JwtToken>.Error("Password is incorrect");
        }

        if (model.Client == null)
            model.Client = detector.GetClientInfo();

        var location = await locationService.GetIpInfoAsync(identityService.GetIP());

        var appDb = new AppModel
        {
            Id = app.Id,
            Name = app.Name,
            ShortName = app.ShortName,
            Image = app.Image,
            Description = app.Description,
            Version = model.App.Version
        };

        var sessionId = Guid.NewGuid();

        var session = new Session
        {
            Id = sessionId,
            IsActive = true,
            App = appDb,
            Client = model.Client,
            Location = location,
            UserId = user.Id
        };

        //var jwtToken = await _tokenService.GetUserTokenAsync(user.Id, sessionId, "pwd");
        var jwtToken = await tokenService.GetUserTokenAsync(new UserTokenModel
        {
            AuthType = "pwd",
            UserId = user.Id,
            Lang = model.Lang,
            SessionId = sessionId
        });

        session.Token = jwtToken.Token;
        session.ExpiredAt = jwtToken.ExpiredAt;
        session.Type = AuthScheme.Password;
        session.ViaMFA = user.MFA;

        session.PrepareToCreate();

        var loginNotify = NotificationsHelper.GetLoginByPasswordNotification(session);
        loginNotify.UserId = user.Id;

        await db.Notifications.AddAsync(loginNotify);

        await db.Sessions.AddAsync(session);

        await db.SaveChangesAsync();

        sessionManager.AddSession(new TokenModel(jwtToken.Token, jwtToken.ExpiredAt));

        if (user.MFA)
            return Result<JwtToken>.MFA(sessionId.ToString());

        return Result<JwtToken>.SuccessWithData(jwtToken);
    }

    public async Task<Result<JwtToken>> LoginBySocialAsync(AuthenticateResult model, string scheme)
    {
        var appResult = await appService.GetAppBySchemeAsync(scheme);
        if (!appResult.IsSuccess)
            return appResult.MapToNew<JwtToken>(null, null);

        var app = appResult.Data;

        if (!app.IsActive)
            return Result<JwtToken>.Error("App already unactive");

        if (!app.IsActiveByTime())
            return Result<JwtToken>.Error("App is expired");

        var claims = model.Principal.Identities.FirstOrDefault()?.Claims;

        var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

        var key = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var userLogin = await db.UserLogins.Include(s => s.User)
            .AsNoTracking().FirstOrDefaultAsync(s =>
                s.ExternalProvider == scheme &&
                s.Email == email &&
                s.Key == key);

        if (userLogin == null)
            return Result<JwtToken>.NotFound("No account linked to this social");

        var user = userLogin.User;

        if (!user.IsActivateAccount)
            return Result<JwtToken>.Error("Account not activated");

        var location = await locationService.GetIpInfoAsync(identityService.GetIP());

        var appDb = new AppModel
        {
            Id = app.Id,
            Name = app.Name,
            ShortName = app.ShortName,
            Image = app.Image,
            Description = app.Description,
            Version = null
        };

        var sessionId = Guid.NewGuid();

        var session = new Session
        {
            Id = sessionId,
            IsActive = true,
            App = appDb,
            Client = detector.GetClientInfo(),
            Location = location,
            UserId = user.Id
        };

        //var jwtToken = await _tokenService.GetUserTokenAsync(user.Id, sessionId, scheme);
        var jwtToken = await tokenService.GetUserTokenAsync(new UserTokenModel
        {
            AuthType = scheme,
            SessionId = sessionId,
            UserId = user.Id,
            Lang = "uk"
        });

        session.Token = jwtToken.Token;
        session.ExpiredAt = jwtToken.ExpiredAt;
        session.Type = scheme;

        session.PrepareToCreate();

        var loginNotify = NotificationsHelper.GetLoginBySocialNotification(session);
        loginNotify.UserId = user.Id;

        await db.Notifications.AddAsync(loginNotify);

        await db.Sessions.AddAsync(session);

        await db.SaveChangesAsync();

        sessionManager.AddSession(new TokenModel(jwtToken.Token, jwtToken.ExpiredAt));

        return Result<JwtToken>.SuccessWithData(jwtToken);
    }

    public async Task<Result<bool>> LogoutAllAsync(int userId)
    {
        var sessions = await db.Sessions.Where(x => x.IsActive && x.UserId == userId).ToListAsync();

        var tokens = sessions.Select(x => x.Token);

        var currentSessionId = identityService.GetCurrentSessionId();

        sessions.ForEach(session =>
        {
            session.IsActive = false;
            session.DeactivatedBySessionId = currentSessionId;
            session.DeactivatedAt = DateTime.Now;
            session.PrepareToUpdate(identityService);
        });

        db.Sessions.UpdateRange(sessions);

        var notify = NotificationsHelper.GetAllLogoutNotification();
        notify.UserId = userId;
        await db.Notifications.AddAsync(notify);
        await db.SaveChangesAsync();

        sessionManager.RemoveRangeSession(tokens);

        return Result<bool>.Success();
    }

    public async Task<Result<bool>> LogoutAsync()
    {
        var token = identityService.GetBearerToken();
        if (!sessionManager.IsActiveSession(token))
            return Result<bool>.Error("Session is already expired");

        var session = await db.Sessions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == identityService.GetCurrentSessionId());
        if (session == null)
            return Result<bool>.NotFound(typeof(Session).NotFoundMessage(identityService.GetCurrentSessionId()));

        var now = DateTime.Now;
        session.IsActive = false;
        session.DeactivatedAt = now;
        session.DeactivatedBySessionId = identityService.GetCurrentSessionId();
        session.PrepareToUpdate(identityService);
        db.Sessions.Update(session);
        await db.SaveChangesAsync();

        sessionManager.RemoveSession(token);

        var notify = NotificationsHelper.GetLogoutNotification(session);
        notify.UserId = identityService.GetUserId();
        await db.Notifications.AddAsync(notify);
        await db.SaveChangesAsync();

        return Result<bool>.SuccessWithData(true);
    }

    public async Task<Result<bool>> LogoutBySessionIdAsync(Guid id)
    {
        var sessionToClose = await db.Sessions.FirstOrDefaultAsync(x => x.Id == id);
        if (sessionToClose == null)
            return Result<bool>.NotFound(typeof(Session).NotFoundMessage(id));
        var now = DateTime.Now;
        sessionToClose.IsActive = false;
        sessionToClose.DeactivatedAt = now;
        sessionToClose.DeactivatedBySessionId = identityService.GetCurrentSessionId();
        sessionToClose.PrepareToUpdate(identityService);
        db.Sessions.Update(sessionToClose);
        var notify = NotificationsHelper.GetLogoutNotification(sessionToClose);
        notify.UserId = sessionToClose.UserId;
        await db.Notifications.AddAsync(notify);
        await db.SaveChangesAsync();
        sessionManager.RemoveSession(sessionToClose.Token);
        return Result<bool>.Success();
    }

    public async Task<Result<AuthenticationInfo>> RegisterAsync(RegisterViewModel model)
    {
        if (await commonService.IsExistAsync<User>(s => s.Login == model.Login))
            return Result<AuthenticationInfo>.Error("Login is busy");

        var groupInvite = await db.GroupInvites.AsNoTracking().FirstOrDefaultAsync(s => s.CodeJoin == model.Code);
        if (groupInvite == null)
            return Result<AuthenticationInfo>.Error("Code isn't exist");

        if (!groupInvite.IsActive)
            return Result<AuthenticationInfo>.Error("Code already unactive");

        if (!groupInvite.IsActiveByTime())
            return Result<AuthenticationInfo>.Error("Code is expired");

        var newUser = new User(model.FirstName, model.MiddleName, model.LastName, model.Login, Generator.GetUsername());
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
        newUser.IsActivateAccount = false;
        newUser.MFA = false;
        newUser.MFASecretKey = null;
        newUser.FromImport = false;
        newUser.ModifiedFromTemp = null;
        newUser.SetLock(true);
        newUser.PrepareToCreate();

        await db.Users.AddAsync(newUser);
        await db.SaveChangesAsync();

        var notify = NotificationsHelper.GetWelcomeNotification();
        notify.UserId = newUser.Id;

        await db.Notifications.AddAsync(notify);
        await db.SaveChangesAsync();

        var role = await db.Roles.AsNoTracking().FirstOrDefaultAsync(s => s.Name == Roles.Student);

        var userRole = new UserRole
        {
            UserId = newUser.Id,
            RoleId = role.Id
        };
        userRole.PrepareToCreate();

        await db.UserRoles.AddAsync(userRole);
        await db.SaveChangesAsync();

        var groupRoleStudent = await db.UserGroupRoles
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UniqId == UserGroupRoles.UniqIds.Student);

        var groupStudent = new UserGroup
        {
            UserId = newUser.Id,
            GroupId = groupInvite.GroupId,
            IsAdmin = false,
            Status = UserGroupStatus.New,
            Title = "Студент",
            UserGroupRoleId = groupRoleStudent.Id
        };

        groupStudent.PrepareToCreate();

        await db.UserGroups.AddAsync(groupStudent);
        await db.SaveChangesAsync();

        return Result<AuthenticationInfo>.Created();
    }

    public async Task<Result<AuthenticationInfo>> RegisterTeacherAsync(RegisterViewModel model)
    {
        if (await commonService.IsExistAsync<User>(s => s.Login == model.Login))
            return Result<AuthenticationInfo>.Error("Login is busy");

        var specialtyByInvite = await db.Specialties.AsNoTracking().FirstOrDefaultAsync(s => s.Invite == model.Code);

        if (specialtyByInvite == null)
            return Result<AuthenticationInfo>.NotFound(typeof(Specialty).NotFoundMessage(model.Code));

        var teacher = new User(model.FirstName, model.MiddleName, model.LastName, model.Login, Generator.GetUsername());

        teacher.PasswordHash = model.Password.GeneratePasswordHash();
        teacher.NotificationSettings = new NotificationSettings
        {
            AcceptedInGroup = true,
            ChangePassword = true,
            Logout = true,
            NewLogin = true,
            NewPost = true,
            Welcome = true
        };

        teacher.SetLock(true);

        teacher.IsActivateAccount = true;
        teacher.MFA = false;
        teacher.MFASecretKey = null;
        teacher.ModifiedFromTemp = null;
        teacher.FromImport = false;

        teacher.PrepareToCreate();

        await db.Users.AddAsync(teacher);
        await db.SaveChangesAsync();

        var notify = NotificationsHelper.GetWelcomeNotification();
        notify.UserId = teacher.Id;

        await db.Notifications.AddAsync(notify);
        await db.SaveChangesAsync();

        var role = await db.Roles.AsNoTracking().FirstOrDefaultAsync(s => s.Name == Roles.Teacher);

        var userRole = new UserRole
        {
            UserId = teacher.Id,
            RoleId = role.Id
        };
        userRole.PrepareToCreate();

        await db.UserRoles.AddAsync(userRole);
        await db.SaveChangesAsync();

        var specialtyTeacher = new UserSpecialty
        {
            UserId = teacher.Id,
            SpecialtyId = specialtyByInvite.Id,
            Title = "Викладач"
        };

        specialtyTeacher.PrepareToCreate();

        await db.UserSpecialties.AddAsync(specialtyTeacher);
        await db.SaveChangesAsync();

        return Result<AuthenticationInfo>.Created();
    }

    public async Task<Result<bool>> SetupUserRolesAsync(UserRoleSetupModel userRole)
    {
        if (!await db.Roles.AnyAsync(s => userRole.RoleIds.Contains(s.Id)))
            return Result<bool>.NotFound($"One from ({string.Join(",", userRole.RoleIds)}) ids not found");

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == userRole.UserId);
        if (user == null)
            return Result<bool>.NotFound(typeof(User).NotFoundMessage(userRole.UserId));

        var userRoles = await db.UserRoles.AsNoTracking().Where(s => s.UserId == userRole.UserId).ToListAsync();

        var res = userRole.RoleIds.SequenceEqual(userRoles.Select(s => s.RoleId));
        if (res)
            return Result<bool>.Success();

        var roleIdsToAdd = userRole.RoleIds.Except(userRoles.Select(s => s.RoleId));

        var newUserRoles = new List<UserRole>();

        foreach (var newRoleId in roleIdsToAdd)
        {
            var newUserRole = new UserRole
            {
                RoleId = newRoleId,
                UserId = user.Id
            };
            newUserRole.PrepareToCreate(identityService);
            newUserRoles.Add(newUserRole);
        }

        await db.UserRoles.AddRangeAsync(newUserRoles);
        await db.SaveChangesAsync();

        var roleIdsToRemove = userRoles.Select(s => s.RoleId).Except(userRole.RoleIds);

        if (roleIdsToRemove != null || roleIdsToRemove.Count() > 0)
        {
            var oldUserRoles = await db.UserRoles.Where(s => s.UserId == user.Id && roleIdsToRemove.Contains(s.RoleId)).ToListAsync();

            db.UserRoles.RemoveRange(oldUserRoles);
            await db.SaveChangesAsync();
        }


        var notification = NotificationsHelper.GetChangeRoleNotification();
        notification.UserId = user.Id;

        notification.PrepareToCreate(identityService);
        await db.Notifications.AddAsync(notification);
        await db.SaveChangesAsync();

        return Result<bool>.Success();
    }

    public async Task<Result<bool>> UnlinkSocialAsync(int socialId)
    {
        var userLoginToRemove = await db.UserLogins.AsNoTracking().FirstOrDefaultAsync(s => s.Id == socialId);
        if (userLoginToRemove == null)
            return Result<bool>.NotFound();

        if (userLoginToRemove.UserId != identityService.GetUserId())
            return Result<bool>.Forbiden();

        db.UserLogins.Remove(userLoginToRemove);
        await db.SaveChangesAsync();
        return Result<bool>.Success();
    }
}
