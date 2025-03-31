using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.University;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class UniversityService(
    IMapper mapper,
    URLSDbContext db,
    IIdentityService identityService,
    ICommonService commonService) : IUniversityService
{
    public async Task<Result<UniversityViewModel>> CreateUniversityAsync(UniversityCreateModel model)
    {
        var count = await commonService.CountAsync<University>();
        if (count > 0)
            return Result<UniversityViewModel>.Error("University is already exist");

        var newUniversity = new University
        {
            Name = model.Name,
            ShortName = model.ShortName,
            NameEng = model.NameEng,
            ShortNameEng = model.ShortNameEng
        };
        newUniversity.PrepareToCreate(identityService);

        await db.Universities.AddAsync(newUniversity);
        await db.SaveChangesAsync();
        return Result<UniversityViewModel>.Created(mapper.Map<UniversityViewModel>(newUniversity));
    }

    public async Task<Result<UniversityViewModel>> GetUniversityAsync()
    {
        var university = await db.Universities.AsNoTracking().FirstOrDefaultAsync();
        if (university == null)
            return Result<UniversityViewModel>.NotFound("University not found");
        var universityViewModel = mapper.Map<UniversityViewModel>(university);
        return Result<UniversityViewModel>.SuccessWithData(universityViewModel);
    }

    public async Task<Result<UniversityViewModel>> UpdateUniversityAsync(UniversityEditModel model)
    {
        var updatedUniversity = await db.Universities.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.Id);
        if (updatedUniversity == null)
            return Result<UniversityViewModel>.NotFound();

        updatedUniversity.Name = model.Name;
        updatedUniversity.ShortName = model.ShortName;
        updatedUniversity.NameEng = model.NameEng;
        updatedUniversity.ShortNameEng = model.ShortNameEng;
        updatedUniversity.PrepareToUpdate(identityService);

        db.Universities.Update(updatedUniversity);
        await db.SaveChangesAsync();
        return Result<UniversityViewModel>.SuccessWithData(mapper.Map<UniversityViewModel>(updatedUniversity));
    }
}
