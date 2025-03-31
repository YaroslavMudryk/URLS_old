using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Apps;
using URLS.Constants;
using URLS.Constants.APIResponse;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class AppService(
    URLSDbContext db,
    IMapper mapper,
    IIdentityService identityService) : IAppService
{
    public async Task<Result<AppViewModel>> ChangeAppSecretAsync(int appId)
    {
        var appForUpdate = await db.Apps.AsNoTracking().FirstOrDefaultAsync(s => s.Id == appId);

        if (appForUpdate == null)
            return Result<AppViewModel>.NotFound(typeof(App).NotFoundMessage(appId));

        if (!identityService.IsAdministrator())
            if (appForUpdate.UserId != identityService.GetUserId())
                return Result<AppViewModel>.Forbiden();

        appForUpdate.AppSecret = Generator.CreateAppSecret();
        appForUpdate.PrepareToUpdate(identityService);

        db.Apps.Update(appForUpdate);
        await db.SaveChangesAsync();

        return Result<AppViewModel>.SuccessWithData(mapper.Map<AppViewModel>(appForUpdate));
    }

    public async Task<Result<AppViewModel>> CreateAppAsync(AppCreateModel app)
    {
        var newApp = mapper.Map<App>(app);

        newApp.AppId = Generator.CreateAppId();
        newApp.AppSecret = Generator.CreateAppSecret();
        newApp.UserId = identityService.GetUserId();
        newApp.PrepareToCreate(identityService);

        await db.Apps.AddAsync(newApp);
        await db.SaveChangesAsync();

        return Result<AppViewModel>.Created(mapper.Map<AppViewModel>(newApp));
    }

    public async Task<Result<bool>> DeleteAppAsync(int id)
    {
        var appForDelete = await db.Apps.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);

        if (appForDelete == null)
            return Result<bool>.NotFound(typeof(App).NotFoundMessage(id));

        if (!identityService.IsAdministrator())
            if (appForDelete.UserId != identityService.GetUserId())
                return Result<bool>.Forbiden();

        db.Apps.Remove(appForDelete);
        await db.SaveChangesAsync();

        return Result<bool>.Success();
    }

    public async Task<Result<List<AppViewModel>>> GetAllAppsAsync(int offset = 0, int limit = 20)
    {
        int count = 0;
        var query = db.Apps.AsNoTracking();

        if (!identityService.IsAdministrator())
        {
            query = query.Where(s => s.UserId == identityService.GetUserId());
            count = await db.Apps.CountAsync(s => s.UserId == identityService.GetUserId());
        }
        else
            count = await db.Apps.CountAsync();

        query = query.OrderBy(s => s.Id).Skip(offset).Take(limit);
        var allApps = await query.ToListAsync();

        var appsToView = mapper.Map<List<AppViewModel>>(allApps);

        return Result<List<AppViewModel>>.SuccessList(appsToView, Meta.FromMeta(count, offset, limit));
    }

    public async Task<Result<AppViewModel>> GetAppByIdAsync(int id)
    {
        var app = await db.Apps.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);

        if (app == null)
            return Result<AppViewModel>.NotFound(typeof(App).NotFoundMessage(id));

        if (!identityService.IsAdministrator())
            if (app.UserId != identityService.GetUserId())
                return Result<AppViewModel>.Forbiden();

        var appToView = mapper.Map<AppViewModel>(app);

        return Result<AppViewModel>.SuccessWithData(appToView);
    }

    public async Task<Result<App>> GetAppBySchemeAsync(string scheme)
    {
        var app = await db.Apps.AsNoTracking().FirstOrDefaultAsync(s => s.Scheme == scheme);
        if (app == null)
            return Result<App>.NotFound(typeof(App).NotFoundMessage(scheme));
        return Result<App>.SuccessWithData(app);
    }

    public async Task<Result<AppDetail>> GetAppDetailsAsync(int id)
    {
        var app = await db.Apps.AsNoTracking().FirstOrDefaultAsync(app => app.Id == id);
        if (app == null)
            return Result<AppDetail>.NotFound(typeof(App).NotFoundMessage(id));

        if (!identityService.IsAdministrator())
            if (app.UserId != identityService.GetUserId())
                return Result<AppDetail>.Forbiden();

        return Result<AppDetail>.SuccessWithData(new AppDetail
        {
            AppId = app.AppId,
            AppSecret = app.AppSecret,
        });
    }

    public async Task<Result<AppViewModel>> UpdateAppAsync(AppEditModel app)
    {
        var appToUpdate = await db.Apps.AsNoTracking().FirstOrDefaultAsync(x => x.Id == app.Id);

        if (appToUpdate == null)
            return Result<AppViewModel>.NotFound(typeof(App).NotFoundMessage(app.Id));

        if (!identityService.IsAdministrator())
            if (appToUpdate.UserId != identityService.GetUserId())
                return Result<AppViewModel>.Forbiden();

        appToUpdate.ActiveFrom = app.ActiveFrom;
        appToUpdate.ActiveTo = app.ActiveTo;
        appToUpdate.Description = app.Description;
        appToUpdate.Image = app.Image;
        appToUpdate.IsActive = app.IsActive;
        appToUpdate.Name = app.Name;
        appToUpdate.ShortName = app.ShortName;
        appToUpdate.PrepareToUpdate(identityService);

        db.Apps.Update(appToUpdate);
        await db.SaveChangesAsync();

        return Result<AppViewModel>.SuccessWithData(mapper.Map<AppViewModel>(appToUpdate));
    }
}
