using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Timetable;
using URLS.Constants.APIResponse;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class TimetableService(
    IIdentityService identityService,
    URLSDbContext db,
    IMapper mapper,
    ICommonService commonService) : ITimetableService
{
    public async Task<Result<TimetableViewModel>> CreateTimetableAsync(TimetableCreateModel model)
    {
        if (!await IsAvailableAsync(model))
            return Result<TimetableViewModel>.Error("This time is busy");
        var setting = await db.Settings.AsNoTracking().FirstOrDefaultAsync();

        var newTimetable = new Timetable
        {
            Date = model.Date,
            SubjectId = model.SubjectId,
            TeacherId = model.TeacherId,
            Type = model.Type,
            GroupId = model.GroupId
        };

        var time = setting.LessonTimes.FirstOrDefault(t => t.Id == model.TimeId);
        if (time != null)
            newTimetable.Time = time;
        else
            return Result<TimetableViewModel>.Error("TimeID not valid value");

        if (model.HolidayId.HasValue)
        {
            var holiday = setting.Holidays.FirstOrDefault(s => s.Id == model.HolidayId.Value);
            if (holiday != null)
            {
                newTimetable.IsHoliday = true;
                newTimetable.Holiday = holiday;
            }
            else
                return Result<TimetableViewModel>.Error("HolidayID not valid value");
        }

        newTimetable.PrepareToCreate(identityService);
        await db.Timetables.AddAsync(newTimetable);
        await db.SaveChangesAsync();

        return Result<TimetableViewModel>.Created(mapper.Map<TimetableViewModel>(newTimetable));
    }

    public async Task<Result<List<TimetableViewModel>>> GetTimetableBetweenDatesAsync(int groupId, DateTime startDate, DateTime endDate)
    {
        if (!await commonService.IsExistAsync<Group>(s => s.Id == groupId))
            return Result<List<TimetableViewModel>>.NotFound("Group not found");

        ValidateDates(ref startDate, ref endDate);

        var timetable = await db.Timetables
            .AsNoTracking()
            .Where(s => s.GroupId == groupId && s.Date >= startDate && s.Date <= endDate)
            .Include(s => s.Teacher)
            .Include(s => s.Subject)
            .OrderBy(s => s.Date)
            .ToListAsync();

        var timetableToView = mapper.Map<List<TimetableViewModel>>(timetable);

        return Result<List<TimetableViewModel>>.SuccessList(timetableToView, Meta.FromMeta(timetable.Count, 0, timetable.Count));
    }

    public async Task<Result<bool>> RemoveTimetableAsync(long[] ids)
    {
        var timetable = await db.Timetables
            .AsNoTracking()
            .Where(s => ids.Contains(s.Id))
            .ToListAsync();

        if (timetable == null || timetable.Count == 0)
            return Result<bool>.NotFound("Items not found");

        db.Timetables.RemoveRange(timetable);
        await db.SaveChangesAsync();
        return Result<bool>.Success();
    }

    public async Task<Result<bool>> RemoveTimetableAsync(int? groupId, int? subjectId, DateTime from, DateTime to)
    {
        ValidateDates(ref from, ref to);

        var query = db.Timetables.AsNoTracking();

        if (groupId.HasValue)
            query = query.Where(s => s.GroupId == groupId);

        if (subjectId.HasValue)
            query = query.Where(s => s.SubjectId == subjectId);

        query = query.Where(s => s.Date >= from && s.Date <= to);

        var timetable = await query.ToListAsync();
        db.Timetables.RemoveRange(timetable);
        await db.SaveChangesAsync();
        return Result<bool>.Success();
    }

    public async Task<Result<TimetableViewModel>> UpdateTimetableAsync(TimetableCreateModel model)
    {
        var currentTimetable = await db.Timetables.AsNoTracking().FirstOrDefaultAsync(s => s.Id == model.Id);
        if (currentTimetable == null)
            return Result<TimetableViewModel>.NotFound("Timetable not found");

        var setting = await db.Settings.AsNoTracking().FirstOrDefaultAsync();

        if (!await IsAvailableAsync(model))
            return Result<TimetableViewModel>.Error("This time is busy");

        currentTimetable.Type = model.Type;
        var time = setting.LessonTimes.FirstOrDefault(t => t.Id == model.TimeId);
        if (time != null)
            currentTimetable.Time = time;
        else
            return Result<TimetableViewModel>.Error("TimeID not valid value");

        if (model.HolidayId.HasValue)
        {
            var holiday = setting.Holidays.FirstOrDefault(s => s.Id == model.HolidayId.Value);
            if (holiday != null)
            {
                currentTimetable.IsHoliday = true;
                currentTimetable.Holiday = holiday;
            }
            else
                return Result<TimetableViewModel>.Error("HolidayID not valid value");
        }

        currentTimetable.PrepareToUpdate(identityService);
        db.Timetables.Update(currentTimetable);
        await db.SaveChangesAsync();

        return Result<TimetableViewModel>.SuccessWithData(mapper.Map<TimetableViewModel>(currentTimetable));
    }

    private void ValidateDates(ref DateTime start, ref DateTime end)
    {
        var diff = end - start;
        if (start > end || diff.Days > 30)
        {
            start = DateTime.Today;
            end = DateTime.Today.AddDays(14);
        }
    }

    private async Task<bool> IsAvailableAsync(TimetableCreateModel model)
    {
        Expression<Func<Timetable, bool>> predicate = (x) =>
        x.GroupId == model.GroupId &&
        x.Date == model.Date;

        var items = await db.Timetables
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();

        var item = items.FirstOrDefault(s => s.Time.Id == model.TimeId);
        if (model.Id.HasValue)
            if (item.Id == model.Id.Value)
                return true;
        if (item == null)
            return true;
        return false;
    }
}
