using AutoMapper;
using Extensions.DeviceDetector;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using URLS.Application.Seeder;
using URLS.Application.Services.Implementations;
using URLS.Application.Services.Interfaces;
using URLS.Constants.Localisation;
using URLS.Infrastructure.Data.Context;

namespace URLS.Infrastructure.IoC;

public static class DependencyContainer
{
    public static IServiceCollection AddURLSServices(this IServiceCollection services, IConfiguration configuration)
    {
        #region Db
        services.AddDbContext<URLSDbContext>(options =>
        {
            //options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"));
            //options.UseSqlite(configuration.GetConnectionString("Sqlite"));
            options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Error);
        });            
        #endregion

        #region Automapper

        services.AddSingleton(new MapperConfiguration(mc =>
        {
            mc.AddProfile(new Application.ViewModels.Mapper());
        }).CreateMapper());

        #endregion

        #region Services

        services.AddScoped<ISeederService, DUTSeederService>();
        services.AddScoped<IUniversityService, UniversityService>();
        services.AddScoped<IIdentityService, HttpIdentityService>();
        services.AddScoped<IFacultyService, FacultyService>();
        services.AddScoped<ISpecialtyService, SpecialtyService>();
        services.AddScoped<IAppService, AppService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IDiplomaService, DiplomaService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IClaimService, ClaimService>();
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<ILessonService, LessonService>();
        services.AddScoped<ITimetableService, TimetableService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IJournalService, JournalService>();
        services.AddScoped<IGroupMemberService, GroupMemberService>();
        services.AddScoped<IGroupInviteService, GroupInviteService>();
        services.AddScoped<IGroupRoleService, GroupRoleService>();
        services.AddScoped<IPermissionPostService, PermissionPostService>();
        services.AddScoped<IPermissionGroupInviteService, PermissionGroupInviteService>();
        services.AddScoped<IPermissionCommentService, PermissionCommentService>();
        services.AddScoped<IQuizService, QuizService>();
        services.AddScoped<ICommonService, CommonService>();
        services.AddScoped<IReactionService, ReactionService>();
        services.AddSingleton<IPushNotificationService, FirebasePushNotificationService>();
        services.AddScoped<IExportService, ExportService>();
        services.AddScoped<IWidgetService, WidgetService>();

        services.AddScoped<ILocalizeService, LocalizeService>();

        services.AddScoped<IImportService, ImportService>();
        #endregion

        #region Managers

        services.AddSingleton<ISessionManager, SessionManager>();
        services.AddScoped<IUserManager, UserManager>();

        #endregion

        #region Common

        services.AddHttpContextAccessor();

        services.AddHttpClient("UaData", httpClient =>
        {
            httpClient.BaseAddress = new Uri("https://uadata.net/");
        });

        services.AddMemoryCache();

        #endregion

        #region Libraries

        services.AddDeviceDetector();

        #endregion

        return services;
    }
}
