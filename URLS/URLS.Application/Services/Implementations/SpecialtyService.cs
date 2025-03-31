using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Specialty;
using URLS.Constants;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class SpecialtyService(
    URLSDbContext db,
    IMapper mapper,
    IIdentityService identityService,
    IFacultyService faсultyService,
    ICommonService commonService) : ISpecialtyService
{
    public async Task<Result<SpecialtyViewModel>> CreateSpecialtyAsync(SpecialtyCreateModel model)
    {
        if (await commonService.IsExistAsync<Specialty>(x => x.Name == model.Name && x.Code == model.Code))
            return Result<SpecialtyViewModel>.Error("Specialty already exist");
        var currentFaculty = await faсultyService.GetFacultyByIdAsync(model.FacultyId);
        if (currentFaculty.IsNotFound)
            return Result<SpecialtyViewModel>.NotFound("Faculty not found");
        var newSpecialty = new Specialty
        {
            Name = model.Name,
            Code = model.Code,
            Invite = Generator.CreateGroupInviteCode(),
            FacultyId = model.FacultyId
        };
        newSpecialty.PrepareToCreate(identityService);
        await db.Specialties.AddAsync(newSpecialty);
        await db.SaveChangesAsync();

        return Result<SpecialtyViewModel>.Created(mapper.Map<SpecialtyViewModel>(newSpecialty));
    }

    public async Task<Result<List<SpecialtyViewModel>>> GetAllSpecialtiesAsync()
    {
        return Result<List<SpecialtyViewModel>>.SuccessList(await db.Specialties.AsNoTracking().Select(x => new SpecialtyViewModel
        {
            Id = x.Id,
            CreatedAt = x.CreatedAt,
            Name = x.Name,
            Code = x.Code
        }).ToListAsync());
    }

    public async Task<Result<string>> GetInviteAsync(int specialtyId)
    {
        var specialty = await db.Specialties.AsNoTracking().FirstOrDefaultAsync(s => s.Id == specialtyId);
        if (specialty == null)
            return Result<string>.NotFound(typeof(Specialty).NotFoundMessage(specialtyId));

        return Result<string>.SuccessWithData(specialty.Invite);
    }

    public async Task<Result<List<SpecialtyViewModel>>> GetSpecialtiesByFacultyIdAsync(int facultyId)
    {
        return Result<List<SpecialtyViewModel>>.SuccessList(await db.Specialties.AsNoTracking().Where(x => x.FacultyId == facultyId).Select(x => new SpecialtyViewModel
        {
            Id = x.Id,
            CreatedAt = x.CreatedAt,
            Name = x.Name,
            Code = x.Code
        }).ToListAsync());
    }

    public async Task<Result<SpecialtyViewModel>> GetSpecialtyByIdAsync(int id)
    {
        var specialty = await db.Specialties.AsNoTracking().Select(x => new SpecialtyViewModel
        {
            Id = x.Id,
            CreatedAt = x.CreatedAt,
            Name = x.Name,
            Code = x.Code
        }).FirstOrDefaultAsync(x => x.Id == id);
        if (specialty == null)
            return Result<SpecialtyViewModel>.NotFound();
        return Result<SpecialtyViewModel>.SuccessWithData(specialty);
    }

    public async Task<Result<string>> UpdateInviteAsync(int specialtyId)
    {
        var specialtyForUpdate = await db.Specialties.AsNoTracking().FirstOrDefaultAsync(s => s.Id == specialtyId);
        if (specialtyForUpdate == null)
            return Result<string>.NotFound(typeof(Specialty).NotFoundMessage(specialtyId));

        specialtyForUpdate.Invite = await GetNewInvite();
        specialtyForUpdate.PrepareToUpdate(identityService);

        db.Specialties.Update(specialtyForUpdate);
        await db.SaveChangesAsync();

        return Result<string>.SuccessWithData(specialtyForUpdate.Invite);
    }

    public async Task<Result<SpecialtyViewModel>> UpdateSpecialtyAsync(SpecialtyEditModel model)
    {
        var currentSpecialty = await db.Specialties.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.Id);
        if (currentSpecialty == null)
            return Result<SpecialtyViewModel>.NotFound();
        var faculty = await db.Faculties.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.FacultyId);
        if (faculty == null)
            return Result<SpecialtyViewModel>.NotFound("Facuty not found");
        currentSpecialty.Name = model.Name;
        currentSpecialty.Code = model.Code;
        currentSpecialty.FacultyId = model.FacultyId;
        currentSpecialty.PrepareToUpdate(identityService);
        db.Specialties.Update(currentSpecialty);
        await db.SaveChangesAsync();
        return Result<SpecialtyViewModel>.SuccessWithData(mapper.Map<SpecialtyViewModel>(currentSpecialty));
    }

    public async Task<Result<List<SpecialtyTeacherViewModel>>> GetSpecialtyTeachersAsync(int specialtyId, int offset, int count)
    {
        if (!await commonService.IsExistAsync<Specialty>(s => s.Id == specialtyId))
            return Result<List<SpecialtyTeacherViewModel>>.NotFound(typeof(Specialty).NotFoundMessage(specialtyId));

        var teachers = await db.UserSpecialties
            .AsNoTracking()
            .Where(s => s.SpecialtyId == specialtyId)
            .Include(s => s.User)
            .OrderBy(s => s.User.LastName)
            .Skip(offset).Take(count)
            .ToListAsync();

        return Result<List<SpecialtyTeacherViewModel>>.SuccessWithData(mapper.Map<List<SpecialtyTeacherViewModel>>(teachers));
    }

    public async Task<Result<SpecialtyTeacherViewModel>> CreateSpecialtyTeacherAsync(SpecialtyTeacherCreateModel createModel)
    {
        if (!await commonService.IsExistAsync<Specialty>(s => s.Id == createModel.SpecialtyId))
            return Result<SpecialtyTeacherViewModel>.NotFound(typeof(Specialty).NotFoundMessage(createModel.SpecialtyId));

        if (await commonService.IsExistAsync<UserSpecialty>(s => s.SpecialtyId == createModel.SpecialtyId && s.UserId == createModel.TeacherId))
            return Result<SpecialtyTeacherViewModel>.Error("This teacher is already member of this specialty");

        var newTeacherSpecialty = new UserSpecialty
        {
            SpecialtyId = createModel.SpecialtyId,
            UserId = createModel.TeacherId,
            Title = createModel.Title
        };

        newTeacherSpecialty.PrepareToCreate(identityService);
        await db.UserSpecialties.AddAsync(newTeacherSpecialty);
        await db.SaveChangesAsync();

        return Result<SpecialtyTeacherViewModel>.SuccessWithData(mapper.Map<SpecialtyTeacherViewModel>(newTeacherSpecialty));
    }

    public async Task<Result<SpecialtyTeacherViewModel>> UpdateSpecialtyTeacherAsync(SpecialtyTeacherEditModel editModel)
    {
        var teacherSpecialtyForUpdate = await db.UserSpecialties.AsNoTracking().FirstOrDefaultAsync(s => s.Id == editModel.Id);
        if (teacherSpecialtyForUpdate == null)
            return Result<SpecialtyTeacherViewModel>.NotFound(typeof(UserSpecialty).NotFoundMessage(editModel.Id));

        teacherSpecialtyForUpdate.Title = editModel.Title;
        teacherSpecialtyForUpdate.PrepareToUpdate(identityService);
        db.UserSpecialties.Update(teacherSpecialtyForUpdate);
        await db.SaveChangesAsync();

        return Result<SpecialtyTeacherViewModel>.SuccessWithData(mapper.Map<SpecialtyTeacherViewModel>(teacherSpecialtyForUpdate));
    }

    public async Task<Result<bool>> RemoveSpecialtyTeacherAsync(int specialtyTeacherId)
    {
        var specialtyTeacherForDelete = await db.UserSpecialties.FirstOrDefaultAsync(s => s.Id == specialtyTeacherId);
        if (specialtyTeacherForDelete == null)
            return Result<bool>.NotFound(typeof(UserSpecialty).NotFoundMessage(specialtyTeacherId));

        db.UserSpecialties.Remove(specialtyTeacherForDelete);
        await db.SaveChangesAsync();

        return Result<bool>.Success();
    }

    private async Task<string> GetNewInvite()
    {
        string invite = Generator.CreateGroupInviteCode();

        while (await db.Specialties.AnyAsync(s => s.Invite == invite))
        {
            invite = Generator.CreateGroupInviteCode();
        }

        return invite;
    }
}
