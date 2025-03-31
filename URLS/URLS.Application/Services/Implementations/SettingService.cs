using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Setting;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class SettingService(
    URLSDbContext db,
    IMapper mapper,
    IIdentityService identityService,
    ICommonService commonService) : ISettingService
{
    public async Task<Result<SettingViewModel>> CreateSettingAsync(SettingCreateModel model)
    {
        if (await commonService.CountAsync<Setting>() >= 1)
            return Result<SettingViewModel>.Error("Can't create more then 1 item");

        var newSetting = new Setting
        {
            FirtsSemesterStart = model.FirtsSemesterStart,
            FirtsSemesterEnd = model.FirtsSemesterEnd,
            SecondSemesterStart = model.SecondSemesterStart,
            SecondSemesterEnd = model.SecondSemesterEnd,
            MaxCourseInUniversity = model.MaxCourseInUniversity,
        };
        newSetting.PrepareToCreate(identityService);

        await db.Settings.AddAsync(newSetting);
        await db.SaveChangesAsync();

        return Result<SettingViewModel>.Created(mapper.Map<SettingViewModel>(newSetting));
    }

    public async Task<Result<Setting>> GetRootSettingAsync()
    {
        var setting = await db.Settings.AsNoTracking().FirstOrDefaultAsync();
        if (setting == null)
            return Result<Setting>.NotFound("Setting not found");
        return Result<Setting>.SuccessWithData(setting);
    }

    public async Task<Result<SettingViewModel>> UpdateSettingAsync(SettingEditModel model)
    {
        var settingToUpdate = await db.Settings.AsNoTracking().FirstOrDefaultAsync(s => s.Id == model.Id);
        if (settingToUpdate == null)
            return Result<SettingViewModel>.NotFound("Setting not found");

        settingToUpdate.MaxCourseInUniversity = model.MaxCourseInUniversity;
        settingToUpdate.FirtsSemesterStart = model.FirtsSemesterStart;
        settingToUpdate.FirtsSemesterEnd = model.FirtsSemesterEnd;
        settingToUpdate.SecondSemesterStart = model.SecondSemesterStart;
        settingToUpdate.SecondSemesterEnd = model.SecondSemesterEnd;
        settingToUpdate.DirectorSignature = model.DirectorSignature;
        settingToUpdate.UniversityStamp = model.UniversityStamp;
        settingToUpdate.Holidays = model.Holidays;
        settingToUpdate.LessonTimes = model.LessonTimes;
        settingToUpdate.PrepareToUpdate(identityService);

        db.Settings.Update(settingToUpdate);
        await db.SaveChangesAsync();

        return Result<SettingViewModel>.SuccessWithData(mapper.Map<SettingViewModel>(settingToUpdate));
    }
}
