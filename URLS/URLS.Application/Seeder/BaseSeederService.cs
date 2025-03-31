using Extensions.Password;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using URLS.Application.Extensions;
using URLS.Application.Helpers;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Identity;
using URLS.Constants;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Seeder;

public class BaseSeederService : ISeederService
{
    protected readonly URLSDbContext _db;
    protected readonly IAuthenticationService _authenticationService;
    protected readonly IConfiguration _configuration;
    protected int _countOfInitEntities;
    private List<Role> InsertedRoles = null;
    private List<Claim> InsertedClaims = null;
    public BaseSeederService(URLSDbContext db, IAuthenticationService authenticationService, IConfiguration configuration)
    {
        _db = db;
        _authenticationService = authenticationService;
        _countOfInitEntities = 0;
        _configuration = configuration;
    }

    public virtual async Task<Result<JwtToken>> SeedSystemAsync()
    {
        if (!await _db.Apps.AnyAsync())
        {
            await _db.Apps.AddRangeAsync(GetApps());
            await _db.SaveChangesAsync();
            _countOfInitEntities++;
        }

        if (!await _db.Settings.AnyAsync())
        {
            await _db.Settings.AddAsync(GetSetting());
            await _db.SaveChangesAsync();
            _countOfInitEntities++;
        }

        if (!await _db.UserGroupRoles.AnyAsync())
        {
            await _db.UserGroupRoles.AddRangeAsync(GetUserGroupRoles());
            await _db.SaveChangesAsync();
            _countOfInitEntities++;
        }

        if (!await _db.Roles.AnyAsync())
        {
            await _db.Roles.AddRangeAsync(GetRoles());
            await _db.SaveChangesAsync();
            InsertedRoles = _db.Roles.Local.ToList();
            _countOfInitEntities++;
        }

        if (!await _db.Claims.AnyAsync())
        {
            await _db.Claims.AddRangeAsync(GetClaims());
            await _db.SaveChangesAsync();
            InsertedClaims = _db.Claims.Local.ToList();
            _countOfInitEntities++;
        }

        if (!await _db.RoleClaims.AnyAsync())
        {
            await _db.RoleClaims.AddRangeAsync(GetRelationsRoleClaim());
            await _db.SaveChangesAsync();
            _countOfInitEntities++;
        }

        if (!await _db.Users.AnyAsync())
        {
            await _db.Users.AddAsync(GetAdmin());
            await _db.SaveChangesAsync();
            await SetupUserRole(_db.Users.Local.FirstOrDefault());
            _countOfInitEntities++;
        }

        if (_countOfInitEntities == 0)
            return Result<JwtToken>.Error("System is already initial");


        return await _authenticationService.LoginByPasswordAsync(GetLoginAdmin());
    }

    private LoginCreateModel GetLoginAdmin()
    {
        return new LoginCreateModel
        {
            Login = GetAdmin().Login,
            Password = Defaults.Password,
            Lang = "uk",
            Client = null,
            App = GetApps().Where(s => s.ShortName == "Web").Select(s => new AppLoginCreateModel { Id = s.AppId, Secret = s.AppSecret, Version = _configuration["App:Version"] }).FirstOrDefault()
        };
    }

    private User GetAdmin()
    {
        var admin = new User("Адмін", "Адмінович", "Адміненко", "admin@mail.com", Generator.GetUsername());
        admin.PasswordHash = Defaults.Password.GeneratePasswordHash();
        admin.LockoutEnabled = false;
        admin.AccessFailedCount = 0;
        admin.JoinAt = DateTime.Now;
        admin.IsActivateAccount = true;
        admin.NotificationSettings = new NotificationSettings
        {
            AcceptedInGroup = true,
            ChangePassword = true,
            NewLogin = true,
            Logout = true,
            NewPost = true,
            Welcome = true
        };
        admin.ModifiedFromTemp = null;
        admin.FromImport = false;
        admin.PrepareToCreate();
        return admin;
    }

    private List<Claim> GetClaims()
    {
        var claims = new List<Claim>();

        #region Apps

        claims.Add(new Claim
        {
            Type = PermissionClaims.Apps,
            Value = Permissions.CanViewAll,
            DisplayName = "Перегляд усіх застосунків (створених користувачем)"
        });
        claims.Add(new Claim
        {
            Type = PermissionClaims.Apps,
            Value = Permissions.CanView,
            DisplayName = "Перегляд застосунку"
        });
        claims.Add(new Claim
        {
            Type = PermissionClaims.Apps,
            Value = Permissions.CanCreate,
            DisplayName = "Створення застосунку"
        });
        claims.Add(new Claim
        {
            Type = PermissionClaims.Apps,
            Value = Permissions.CanEdit,
            DisplayName = "Оновлення застосунку"
        });
        claims.Add(new Claim
        {
            Type = PermissionClaims.Apps,
            Value = Permissions.CanRemove,
            DisplayName = "Видалення застосунку"
        });

        #endregion

        #region Diplomas

        claims.Add(new Claim
        {
            DisplayName = "Перегляд диплому",
            Type = PermissionClaims.Diplomas,
            Value = Permissions.CanView
        });
        claims.Add(new Claim
        {
            DisplayName = "Перегляд усіх дипломів",
            Type = PermissionClaims.Diplomas,
            Value = Permissions.CanViewAll
        });
        claims.Add(new Claim
        {
            DisplayName = "Створення диплому",
            Type = PermissionClaims.Diplomas,
            Value = Permissions.CanCreate
        });

        #endregion

        #region Faculties

        claims.Add(new Claim
        {
            DisplayName = "Перегляд усіх факультетів (інститутів)",
            Type = PermissionClaims.Faculties,
            Value = Permissions.CanViewAll
        });
        claims.Add(new Claim
        {
            DisplayName = "Перегляд факультету (інституту)",
            Type = PermissionClaims.Faculties,
            Value = Permissions.CanView
        });
        claims.Add(new Claim
        {
            DisplayName = "Перегляд усіх спеціальностей прив'язаних до факультету (інституту)",
            Type = PermissionClaims.Faculties,
            Value = Permissions.CanViewAllSpecialties
        });
        claims.Add(new Claim
        {
            DisplayName = "Створення факультету (інституту)",
            Type = PermissionClaims.Faculties,
            Value = Permissions.CanCreate
        });
        claims.Add(new Claim
        {
            DisplayName = "Оновлення факультету (інституту)",
            Type = PermissionClaims.Faculties,
            Value = Permissions.CanEdit
        });

        #endregion

        #region Identity

        claims.Add(new Claim
        {
            DisplayName = "Створення, перегляд, редагування, видалення, ролей та дозволів",
            Type = PermissionClaims.Identity,
            Value = Permissions.All,
        });

        #endregion

        #region Settings

        claims.Add(new Claim
        {
            DisplayName = "Перегляд налаштувань застосунку",
            Type = PermissionClaims.Settings,
            Value = Permissions.CanView
        });
        claims.Add(new Claim
        {
            DisplayName = "Створення налаштувань застосунку",
            Type = PermissionClaims.Settings,
            Value = Permissions.CanCreate
        });
        claims.Add(new Claim
        {
            DisplayName = "Оновлення налаштувань застосунку",
            Type = PermissionClaims.Settings,
            Value = Permissions.CanEdit
        });

        #endregion

        #region Specialties

        claims.Add(new Claim
        {
            DisplayName = "Перегляд усіх спеціальностей",
            Type = PermissionClaims.Specialties,
            Value = Permissions.CanViewAll
        });
        claims.Add(new Claim
        {
            DisplayName = "Перегляд спеціальності",
            Type = PermissionClaims.Specialties,
            Value = Permissions.CanView
        });
        claims.Add(new Claim
        {
            DisplayName = "Перегляд усіх груп",
            Type = PermissionClaims.Specialties,
            Value = Permissions.CanViewAllGroups
        });
        claims.Add(new Claim
        {
            DisplayName = "Створення спеціальності",
            Type = PermissionClaims.Specialties,
            Value = Permissions.CanCreate
        });
        claims.Add(new Claim
        {
            DisplayName = "Оновлення спеціальності",
            Type = PermissionClaims.Specialties,
            Value = Permissions.CanEdit
        });

        #endregion

        #region Timetable

        claims.Add(new Claim
        {
            DisplayName = "Перегляд розкладу",
            Type = PermissionClaims.Timetable,
            Value = Permissions.CanView
        });
        claims.Add(new Claim
        {
            DisplayName = "Створення розкладу",
            Type = PermissionClaims.Timetable,
            Value = Permissions.CanCreate
        });
        claims.Add(new Claim
        {
            DisplayName = "Оновлення розкладу",
            Type = PermissionClaims.Timetable,
            Value = Permissions.CanEdit
        });
        claims.Add(new Claim
        {
            DisplayName = "Видалення розкладу",
            Type = PermissionClaims.Timetable,
            Value = Permissions.CanRemove
        });

        #endregion

        #region Universities

        claims.Add(new Claim
        {
            DisplayName = "Перегляд університету",
            Type = PermissionClaims.University,
            Value = Permissions.CanView
        });
        claims.Add(new Claim
        {
            DisplayName = "Перегляд факультетів унівреситету",
            Type = PermissionClaims.University,
            Value = Permissions.CanViewAll
        });
        claims.Add(new Claim
        {
            DisplayName = "Створення унівреситету",
            Type = PermissionClaims.University,
            Value = Permissions.CanCreate
        });
        claims.Add(new Claim
        {
            DisplayName = "Оновлення унівреситету",
            Type = PermissionClaims.University,
            Value = Permissions.CanEdit
        });

        #endregion

        claims.ForEach(item =>
        {
            item.PrepareToCreate();
        });
        return claims;
    }

    public List<RoleClaim> GetRelationsRoleClaim()
    {
        #region ConditionalForWhere

        Func<Claim, bool> _getConditionalForModerator = (Claim s) =>
            s.Type == PermissionClaims.Apps
            || s.Type == PermissionClaims.Diplomas
            || s.Type == PermissionClaims.University
            || s.Type == PermissionClaims.Faculties
            || s.Type == PermissionClaims.Specialties
            || s.Type == PermissionClaims.Timetable
            || s.Type == PermissionClaims.Identity
            || s.Type == PermissionClaims.Settings;


        Func<Claim, bool> _getConditionalForDeveloper = (Claim s) =>
            s.Type == PermissionClaims.Apps
            || s.Type == PermissionClaims.University && s.Value == Permissions.CanView
            || s.Type == PermissionClaims.University && s.Value == Permissions.CanViewAll
            || s.Type == PermissionClaims.Faculties && s.Value == Permissions.CanViewAll
            || s.Type == PermissionClaims.Faculties && s.Value == Permissions.CanView
            || s.Type == PermissionClaims.Faculties && s.Value == Permissions.CanViewAllSpecialties
            || s.Type == PermissionClaims.Specialties && s.Value == Permissions.CanViewAll
            || s.Type == PermissionClaims.Specialties && s.Value == Permissions.CanView
            || s.Type == PermissionClaims.Specialties && s.Value == Permissions.CanViewAllGroups
            || s.Type == PermissionClaims.Settings && s.Value == Permissions.CanView;

        Func<Claim, bool> _getConditionalForTeacher = (Claim s) =>
            s.Type == PermissionClaims.Timetable
            || s.Type == PermissionClaims.University && s.Value == Permissions.CanView
            || s.Type == PermissionClaims.University && s.Value == Permissions.CanViewAll
            || s.Type == PermissionClaims.Faculties && s.Value == Permissions.CanViewAll
            || s.Type == PermissionClaims.Faculties && s.Value == Permissions.CanView
            || s.Type == PermissionClaims.Faculties && s.Value == Permissions.CanViewAllSpecialties
            || s.Type == PermissionClaims.Specialties && s.Value == Permissions.CanViewAll
            || s.Type == PermissionClaims.Specialties && s.Value == Permissions.CanView
            || s.Type == PermissionClaims.Specialties && s.Value == Permissions.CanViewAllGroups
            || s.Type == PermissionClaims.Settings && s.Value == Permissions.CanView;

        Func<Claim, bool> _getConditionalForStudent = (Claim s) =>
            s.Type == PermissionClaims.University && s.Value == Permissions.CanView
            || s.Type == PermissionClaims.University && s.Value == Permissions.CanViewAll
            || s.Type == PermissionClaims.Faculties && s.Value == Permissions.CanViewAll
            || s.Type == PermissionClaims.Faculties && s.Value == Permissions.CanView
            || s.Type == PermissionClaims.Faculties && s.Value == Permissions.CanViewAllSpecialties
            || s.Type == PermissionClaims.Specialties && s.Value == Permissions.CanViewAll
            || s.Type == PermissionClaims.Specialties && s.Value == Permissions.CanView
            || s.Type == PermissionClaims.Specialties && s.Value == Permissions.CanViewAllGroups
            || s.Type == PermissionClaims.Settings && s.Value == Permissions.CanView
            || s.Type == PermissionClaims.Timetable && s.Value == Permissions.CanView;

        #endregion


        var roleClaims = new List<RoleClaim>();
        var roles = InsertedRoles;
        var claims = InsertedClaims;

        foreach (var role in roles)
        {
            if (role.Name == Roles.Admin)
            {
                claims.ForEach(claim =>
                {
                    roleClaims.Add(new RoleClaim
                    {
                        ClaimId = claim.Id,
                        RoleId = role.Id
                    });
                });

            }
            if (role.Name == Roles.Moderator)
            {
                foreach (var claim in claims.Where(_getConditionalForModerator))
                {
                    roleClaims.Add(new RoleClaim
                    {
                        ClaimId = claim.Id,
                        RoleId = role.Id
                    });
                }
            }
            if (role.Name == Roles.Developer)
            {
                foreach (var claim in claims.Where(_getConditionalForDeveloper))
                {
                    roleClaims.Add(new RoleClaim
                    {
                        ClaimId = claim.Id,
                        RoleId = role.Id
                    });
                }
            }
            if (role.Name == Roles.Teacher)
            {
                foreach (var claim in claims.Where(_getConditionalForTeacher))
                {
                    roleClaims.Add(new RoleClaim
                    {
                        ClaimId = claim.Id,
                        RoleId = role.Id
                    });
                }
            }
            if (role.Name == Roles.Student)
            {
                foreach (var claim in claims.Where(_getConditionalForStudent))
                {
                    roleClaims.Add(new RoleClaim
                    {
                        ClaimId = claim.Id,
                        RoleId = role.Id
                    });
                }
            }
        }

        roleClaims.ForEach(item =>
        {
            item.PrepareToCreate();
        });
        return roleClaims;
    }

    private List<Role> GetRoles()
    {
        var listRoles = new List<Role>();

        var role1 = new Role(Roles.Admin);
        role1.Label = Roles.AdminUa;
        role1.Color = Roles.AdminColor;
        role1.CanDelete = false;
        listRoles.Add(role1);

        var role2 = new Role(Roles.Moderator);
        role2.Label = Roles.ModeratorUa;
        role2.Color = Roles.ModeratorColor;
        role2.CanDelete = false;
        listRoles.Add(role2);

        var role3 = new Role(Roles.Developer);
        role3.Label = Roles.DeveloperUa;
        role3.Color = Roles.DeveloperColor;
        role3.CanDelete = false;
        listRoles.Add(role3);

        var role4 = new Role(Roles.Teacher);
        role4.Label = Roles.TeacherUa;
        role4.Color = Roles.TeacherColor;
        role4.CanDelete = false;
        listRoles.Add(role4);

        var role5 = new Role(Roles.Student);
        role5.Label = Roles.StudentUa;
        role5.Color = Roles.StudentColor;
        role5.CanDelete = false;
        listRoles.Add(role5);

        listRoles.ForEach(item =>
        {
            item.PrepareToCreate();
        });
        return listRoles;
    }

    private Setting GetSetting()
    {
        var now = DateTime.Now;

        var newSetting = new Setting
        {
            MaxCourseInUniversity = 6,
            FirtsSemesterStart = new DateTime(now.Year, 9, 1),
            FirtsSemesterEnd = new DateTime(now.Year, 12, 31),
            SecondSemesterStart = new DateTime(now.Year + 1, 1, 1),
            SecondSemesterEnd = new DateTime(now.Year + 1, 6, 30),
            Holidays = new List<Holiday>()
                {
                    new Holiday { Id = 1, Name = "Новий Рік", Date = new DateTime(2022, 1, 1) },
                    new Holiday { Id = 2, Name = "Різдво Христове", Date = new DateTime(2022, 1, 7) },
                    new Holiday { Id = 3, Name = "Міжнародний жіночий день", Date = new DateTime(2022, 3, 8) },
                    new Holiday { Id = 4, Name = "Великдень", Date = new DateTime(2022, 4, 24) },
                    new Holiday { Id = 5, Name = "День праці", Date = new DateTime(2022, 5, 1) },
                    new Holiday { Id = 6, Name = "День пам'яті", Date = new DateTime(2022, 5, 9) },
                    new Holiday { Id = 7, Name = "Трійця", Date = new DateTime(2022, 6,12) },
                    new Holiday { Id = 8, Name = "День Конституції України", Date = new DateTime(2022, 6, 28) },
                    new Holiday { Id = 9, Name = "День Незалежності України", Date = new DateTime(2022, 8, 24) },
                    new Holiday { Id = 10, Name = "День Захисники України", Date = new DateTime(2022, 10, 14) },
                    new Holiday { Id = 11, Name = "Різдво Христове за Григоріанським календарем", Date = new DateTime(2022, 12, 25) },

                }.AsEnumerable(),
            LessonTimes = new List<LessonTime>
                {
                    new LessonTime
                    {
                        Id = 1,
                        Start = new DateTime(2022, 01, 01, 8,0, 0),
                        End = new DateTime(2022, 01, 01, 9, 35, 0),
                        Description = "Перша зміна",
                        Number = 1.ToString()
                    },
                    new LessonTime
                    {
                        Id = 2,
                        Start = new DateTime(2022, 01, 01, 9,45,0),
                        End = new DateTime(2022, 01, 01, 11, 20, 0),
                        Description = "Перша зміна",
                        Number = 2.ToString()
                    },
                    new LessonTime
                    {
                        Id = 3,
                        Start = new DateTime(2022, 01, 01, 11,45, 0),
                        End = new DateTime(2022, 01, 01, 13, 20, 0),
                        Description = "Перша зміна",
                        Number = 3.ToString()
                    },
                    new LessonTime
                    {
                        Id = 4,
                        Start = new DateTime(2022, 01, 01, 13,30,0),
                        End = new DateTime(2022, 01, 01, 15, 05, 0),
                        Description = "Друга зміна",
                        Number = 4.ToString()
                    },
                    new LessonTime
                    {
                        Id = 5,
                        Start = new DateTime(2022, 01, 01, 15,15, 0),
                        End = new DateTime(2022, 01, 01, 16, 50, 0),
                        Description = "Друга зміна",
                        Number = 5.ToString()
                    },
                    new LessonTime
                    {
                        Id = 6,
                        Start = new DateTime(2022, 01, 01, 17,0, 0),
                        End = new DateTime(2022, 01, 01, 18, 35, 0),
                        Description = "Друга зміна",
                        Number = 6.ToString()
                    }
                }.AsEnumerable()
        };
        newSetting.PrepareToCreate();

        return newSetting;
    }

    private List<App> GetApps()
    {
        var listApps = new List<App>();

        var app1 = new App
        {
            Name = "Веб застосунок",
            ShortName = "Web",
            AppId = "x3Vw-QFdk-Pt0B-7jjd",
            AppSecret = "DlST6311QjEfEoUX0JJAsjQGmHrBlUl8qjhXMCaJFJrlTiv0Fn0PcWqOPCEfsrEZuSfvMj",
            ActiveFrom = DateTime.Now,
            ActiveTo = DateTime.Now.AddYears(10),
            Description = "Веб застосунок для роботи з системою УСДН",
            IsActive = true,
            Image = "/files/Web.png",
            Scheme = null
        };
        listApps.Add(app1);

        var app2 = new App
        {
            Name = "Android застосунок",
            ShortName = "Android",
            AppId = "F7Gv-2mu8-rH7Z-U4pt",
            AppSecret = "bUqNPgQhJoj8oHyIsz2cwICuxTCGo0oBLDaBODiQ0pOLnLn6H99IEJipavIwHhbEzf5Y4s",
            ActiveFrom = DateTime.Now,
            ActiveTo = DateTime.Now.AddYears(10),
            Description = "Android застосунок для роботи з системою УСДН",
            IsActive = true,
            Image = "/files/Android.png",
            Scheme = null
        };
        listApps.Add(app2);

        var app3 = new App
        {
            Name = "IOS застосунок",
            ShortName = "IOS",
            AppId = "5BoG-NGOS-Ofqu-6mmS",
            AppSecret = "WI6IygntT3a7ur39Ndv6w6bsvWYYz31PMg6HdKfKzz3Wsi8q4L5aA65hwfzZZqcxOkahf6",
            ActiveFrom = DateTime.Now,
            ActiveTo = DateTime.Now.AddYears(10),
            Description = "IOS застосунок для роботи з системою УСДН",
            IsActive = true,
            Image = "/files/Apple.png",
            Scheme = null
        };
        listApps.Add(app3);

        var app4 = new App
        {
            Name = "Desktop застосунок",
            ShortName = "Desktop",
            AppId = "T2Ke-RJUB-lV0u-BHpB",
            AppSecret = "cc8Qa7fILdAmmxiZszfaxzMUrJSPZKGagVxOqvhmyWwxznGHKyNpGLBOzpOBxdoOHyxcRY",
            ActiveFrom = DateTime.Now,
            ActiveTo = DateTime.Now.AddYears(10),
            Description = "Desktop застосунок для роботи з системою УСДН",
            IsActive = true,
            Image = "/files/Windows.png",
            Scheme = null
        };
        listApps.Add(app4);

        var app5 = new App
        {
            Name = "Окремий застосунок",
            ShortName = "Other",
            AppId = "T2KU-RJUB-lV0u-BHpB",
            AppSecret = "cc8Qa7fILdAmmxiZszfaxzMUrJSPZKGagVxOqvhmyWwxznGHKyNpGLBOzpOBxdoGHyxcRY",
            ActiveFrom = DateTime.Now,
            ActiveTo = DateTime.Now.AddYears(10),
            Description = "Окремий застосунок для роботи з системою УСДН",
            IsActive = true,
            Image = "/files/Other.png",
            Scheme = null
        };
        listApps.Add(app5);

        listApps.ForEach(item =>
        {
            item.PrepareToCreate();
        });
        return listApps;
    }

    private List<UserGroupRole> GetUserGroupRoles()
    {
        var listUserGroupRole = new List<UserGroupRole>();

        var groupRole1 = new UserGroupRole
        {
            Name = UserGroupRoles.Names.ClassTeacher,
            NameEng = UserGroupRoles.Names.ClassTeacherEng,
            Description = UserGroupRoles.Descriptions.ClassTeacherDes,
            DescriptionEng = UserGroupRoles.Descriptions.ClassTeacherEngDes,
            Color = UserGroupRoles.Colors.Red,
            Permissions = new UserGroupPermission
            {
                CanCreateInviteCode = true,
                CanViewInviteCodes = true,
                CanRemoveInviteCode = true,
                CanCreateComment = true,
                CanUpdateImage = true,
                CanRemovePost = true,
                CanRemoveComment = true,
                CanCreatePost = true,
                CanUpdateAllPosts = true,
                CanUpdatePost = true,
                CanOpenCloseComment = true,
                CanRemoveAllComments = true,
                CanRemoveAllPosts = true,
                CanUpdateInviteCode = true,
                CanEditInfo = true
            },
            CanEdit = false,
            UniqId = UserGroupRoles.UniqIds.ClassTeacher
        };
        listUserGroupRole.Add(groupRole1);

        var groupRole2 = new UserGroupRole
        {
            Name = UserGroupRoles.Names.Headmaster,
            NameEng = UserGroupRoles.Names.HeadmasterEng,
            Description = UserGroupRoles.Descriptions.HeadmasterDes,
            DescriptionEng = UserGroupRoles.Descriptions.HeadmasterEngDes,
            Color = UserGroupRoles.Colors.Green,
            Permissions = new UserGroupPermission
            {
                CanCreateInviteCode = true,
                CanViewInviteCodes = true,
                CanRemoveInviteCode = true,
                CanCreateComment = true,
                CanUpdateImage = true,
                CanRemovePost = true,
                CanRemoveComment = true,
                CanCreatePost = true,
                CanUpdateAllPosts = false,
                CanUpdatePost = true,
                CanOpenCloseComment = true,
                CanRemoveAllComments = false,
                CanRemoveAllPosts = false,
                CanUpdateInviteCode = true,
                CanEditInfo = false
            },
            CanEdit = false,
            UniqId = UserGroupRoles.UniqIds.Headmaster
        };
        listUserGroupRole.Add(groupRole2);

        var groupRole3 = new UserGroupRole
        {
            Name = UserGroupRoles.Names.Student,
            NameEng = UserGroupRoles.Names.StudentEng,
            Description = UserGroupRoles.Descriptions.StudentDes,
            DescriptionEng = UserGroupRoles.Descriptions.StudentEngDes,
            Color = UserGroupRoles.Colors.Blue,
            Permissions = new UserGroupPermission
            {
                CanCreateInviteCode = false,
                CanViewInviteCodes = false,
                CanRemoveInviteCode = false,
                CanCreateComment = true,
                CanUpdateImage = false,
                CanRemovePost = false,
                CanRemoveComment = true,
                CanCreatePost = false,
                CanUpdateAllPosts = false,
                CanUpdatePost = false,
                CanOpenCloseComment = false,
                CanRemoveAllComments = false,
                CanRemoveAllPosts = false,
                CanUpdateInviteCode = false,
                CanEditInfo = false
            },
            CanEdit = false,
            UniqId = UserGroupRoles.UniqIds.Student
        };
        listUserGroupRole.Add(groupRole3);

        listUserGroupRole.ForEach(item =>
        {
            item.PrepareToCreate();
        });
        return listUserGroupRole;
    }

    private async Task SetupUserRole(User user)
    {
        var welcomeNotification = NotificationsHelper.GetWelcomeNotification();
        welcomeNotification.UserId = user.Id;
        welcomeNotification.PrepareToCreate();


        var userRoles = await _db.Roles.AsNoTracking().Where(s => s.Name == Roles.Admin || s.Name == Roles.Teacher).ToListAsync();

        IList<UserRole> dbRoles = new List<UserRole>();

        foreach(var role in userRoles)
        {
            var newUserRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            newUserRole.PrepareToCreate();
            dbRoles.Add(newUserRole);
        }

        await _db.Notifications.AddAsync(welcomeNotification);
        await _db.UserRoles.AddRangeAsync(dbRoles);
        await _db.SaveChangesAsync();
    }
}
