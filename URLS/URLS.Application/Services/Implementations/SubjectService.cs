using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using URLS.Application.Extensions;
using URLS.Application.Options;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Subject;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class SubjectService(
    IIdentityService identityService,
    IMapper mapper,
    URLSDbContext db,
    ICommonService commonService) : ISubjectService
{
    public async Task<Result<SubjectViewModel>> CreateSubjectAsync(SubjectCreateModel model)
    {
        if (!await commonService.IsExistAsync<User>(s => s.Id == model.TeacherId))
            return Result<SubjectViewModel>.NotFound("Teacher with this ID not found");

        if (model.GroupId != null)
            if (!await commonService.IsExistAsync<Group>(s => s.Id == model.GroupId))
                return Result<SubjectViewModel>.NotFound($"Group with ID ({model.GroupId}) not found");

        var newSubject = new Subject
        {
            Name = model.Name,
            Description = model.Description,
            From = model.From,
            To = model.To,
            Config = model.Config,
            Semestr = model.Semestr,
            IsTemplate = model.IsTemplate,
            TeacherId = model.TeacherId,
            GroupId = model.GroupId
        };
        newSubject.PrepareToCreate(identityService);
        await db.Subjects.AddAsync(newSubject);
        await db.SaveChangesAsync();
        return Result<SubjectViewModel>.Created(mapper.Map<SubjectViewModel>(newSubject));
    }

    public async Task<Result<List<SubjectViewModel>>> SearchSubjectsAsync(SearchSubjectOptions options)
    {
        options.PrepareOptions();

        var setting = await db.Settings.AsNoTracking().FirstOrDefaultAsync();

        var query = db.Subjects.AsNoTracking();

        if (options.GroupId != null)
            query = query.Where(s => s.GroupId == options.GroupId);

        if (options.IsTemplate != null)
            query = query.Where(x => x.IsTemplate);

        if (options.IsCurrentSemestr != null && options.IsCurrentSemestr.Value)
            query = query.Where(GetCurrentSemestr(setting));

        if (options.Name != null)
            query = query.Where(x => x.Name.Contains(options.Name));

        if (options.SubjectIds != null)
            query = query.Where(s => options.SubjectIds.Contains(s.Id));

        query = query.Skip(options.Offset).Take(options.Count);

        query = query.OrderBy(x => x.Id);

        query = query.Include(s => s.Teacher);

        var subjects = await query.ToListAsync();
        var subjectsToView = mapper.Map<List<SubjectViewModel>>(subjects);
        return Result<List<SubjectViewModel>>.SuccessList(subjectsToView);
    }

    public async Task<Result<SubjectViewModel>> GetSubjectByIdAsync(int subjectId)
    {
        var subject = await db.Subjects
            .AsNoTracking()
            .Include(x => x.Teacher)
            .Include(x => x.Group)
            .FirstOrDefaultAsync(s => s.Id == subjectId);
        if (subject == null)
            return Result<SubjectViewModel>.NotFound("Subject not found");
        var subjectToView = mapper.Map<SubjectViewModel>(subject);
        return Result<SubjectViewModel>.SuccessWithData(subjectToView);
    }

    private Expression<Func<Subject, bool>> GetCurrentSemestr(Setting setting)
    {
        var today = DateTime.Today;
        if (today.Month >= setting.FirtsSemesterStart.Month && today.Month <= setting.FirtsSemesterEnd.Month)
        {
            return (x) => x.From.Month >= setting.FirtsSemesterStart.Month && x.To.Month <= setting.FirtsSemesterEnd.Month;
        }
        if (today.Month >= setting.SecondSemesterStart.Month && today.Month <= setting.SecondSemesterEnd.Month)
        {
            return (x) => x.From.Month >= setting.SecondSemesterStart.Month && x.To.Month <= setting.SecondSemesterEnd.Month;
        }
        return null;
    }

    public async Task<Result<SubjectViewModel>> UpdateSubjectAsync(SubjectEditModel model)
    {
        var query = await commonService.IsExistWithResultsAsync<Subject>(s => s.Id == model.Id);
        if (!query.IsExist)
            return Result<SubjectViewModel>.NotFound("Subject not found");

        if (!await commonService.IsExistAsync<User>(s => s.Id == model.TeacherId))
            return Result<SubjectViewModel>.NotFound("Teacher with this ID not found");

        if (model.GroupId != null)
            if (!await commonService.IsExistAsync<Group>(s => s.Id == model.GroupId))
                return Result<SubjectViewModel>.NotFound($"Group with ID ({model.GroupId}) not found");

        var subjectToUpdate = query.Results.First();

        subjectToUpdate.GroupId = model.GroupId;
        subjectToUpdate.TeacherId = model.TeacherId;

        subjectToUpdate.Name = model.Name;
        subjectToUpdate.Description = model.Description;
        subjectToUpdate.Config = model.Config;
        subjectToUpdate.From = model.From;
        subjectToUpdate.To = model.To;
        subjectToUpdate.IsTemplate = model.IsTemplate;
        subjectToUpdate.Semestr = model.Semestr;

        subjectToUpdate.PrepareToUpdate(identityService);

        db.Subjects.Update(subjectToUpdate);
        await db.SaveChangesAsync();

        return Result<SubjectViewModel>.SuccessWithData(mapper.Map<SubjectViewModel>(subjectToUpdate));
    }

    public async Task<Result<SubjectViewModel>> GetGroupSubjectAsync(int groupId, int subjectId)
    {
        if (!await commonService.IsExistAsync<Group>(s => s.Id == groupId))
            return Result<SubjectViewModel>.NotFound($"Group with ID ({groupId}) not found");

        var searchResult = await SearchSubjectsAsync(new SearchSubjectOptions
        {
            GroupId = groupId,
            SubjectIds = new[] { subjectId }
        });
        if (!searchResult.Data.Any())
            return Result<SubjectViewModel>.NotFound("Subject not found");

        return Result<SubjectViewModel>.SuccessWithData(searchResult.Data[0]);
    }
}
