using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Options;
using URLS.Application.Services.Interfaces;
using URLS.Application.Validations;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group;
using URLS.Application.ViewModels.Group.GroupMember;
using URLS.Constants;
using URLS.Constants.APIResponse;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class GroupService(
    URLSDbContext db,
    IMapper mapper,
    IIdentityService identityService,
    ICommonService commonService) : IGroupService
{
    public async Task<Result<GroupViewModel>> CreateGroupAsync(GroupCreateModel model)
    {
        if (await commonService.IsExistAsync<Group>(s => s.Name == model.Name && s.StartStudy == model.StartStudy))
            return Result<GroupViewModel>.Error("Same group already exist");

        if (!await commonService.IsExistAsync<Specialty>(s => s.Id == model.SpecialtyId))
            return Result<GroupViewModel>.NotFound(typeof(Specialty).NotFoundMessage(model.SpecialtyId));

        if (!model.TryValidateGroupName(out var error))
        {
            return Result<GroupViewModel>.Error(error);
        }

        var newGroup = new Group
        {
            Name = model.Name,
            Course = model.Course,
            StartStudy = model.StartStudy,
            EndStudy = model.EndStudy,
            SpecialtyId = model.SpecialtyId
        };

        if (model.TryGetIndexForNumber(out var index))
        {
            newGroup.IndexNumber = index;
        }

        newGroup.PrepareToCreate(identityService);

        await db.Groups.AddAsync(newGroup);
        await db.SaveChangesAsync();

        var newInvite = new GroupInvite
        {
            ActiveFrom = Defaults.GroupInviteActiveFrom,
            ActiveTo = Defaults.GroupInviteActiveTo,
            CodeJoin = Generator.CreateGroupInviteCode(),
            GroupId = newGroup.Id,
            IsActive = true,
            Name = "Головне запрошення"
        };
        newInvite.PrepareToCreate(identityService);

        await db.GroupInvites.AddAsync(newInvite);
        await db.SaveChangesAsync();

        if (model.ClassTeacherId.HasValue)
        {
            var userGroupRole = await db.UserGroupRoles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UniqId == UserGroupRoles.UniqIds.ClassTeacher);

            var groupMember = new UserGroup
            {
                GroupId = newGroup.Id,
                UserId = model.ClassTeacherId.Value,
                IsAdmin = true,
                Status = UserGroupStatus.Member,
                Title = UserGroupRoles.Names.ClassTeacher,
                UserGroupRoleId = userGroupRole.Id,
            };
            groupMember.PrepareToCreate(identityService);
            await db.UserGroups.AddAsync(groupMember);
            await db.SaveChangesAsync();
        }

        var groupFromDb = await db.Groups.AsNoTracking().FirstOrDefaultAsync(x => x.Id == newGroup.Id);

        var groupToView = mapper.Map<GroupViewModel>(groupFromDb);

        groupToView.CountOfStudents = await db.UserGroups.CountAsync(x => x.GroupId == newGroup.Id && x.Status == Domain.Models.UserGroupStatus.Member);

        groupToView.GroupInvites = await db.GroupInvites.AsNoTracking().Where(x => x.GroupId == newGroup.Id).Select(s => new GroupInviteViewModel
        {
            Id = s.Id,
            CreatedAt = s.CreatedAt,
            Name = s.Name,
            CodeJoin = s.CodeJoin,
            ActiveFrom = s.ActiveFrom,
            ActiveTo = s.ActiveTo,
            IsActive = s.IsActive
        }).ToListAsync();

        return Result<GroupViewModel>.Created(groupToView);
    }

    public async Task<Result<List<GroupViewModel>>> GetAllGroupsAsync(int offset = 0, int limit = 20)
    {
        var groups = await db.Groups
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .Skip(offset).Take(limit)
            .ToListAsync();
        if (groups == null || groups.Count == 0)
            return Result<List<GroupViewModel>>.Success();

        var totalCount = await db.Groups.CountAsync();
        var groupsToView = mapper.Map<List<GroupViewModel>>(groups);

        return Result<List<GroupViewModel>>.SuccessList(groupsToView, Meta.FromMeta(totalCount, offset, limit));
    }

    public async Task<Result<GroupViewModel>> GetGroupByIdAsync(int id)
    {
        var group = await db.Groups.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);

        if (group == null)
            return Result<GroupViewModel>.NotFound(typeof(Group).NotFoundMessage(id));

        var groupToView = mapper.Map<GroupViewModel>(group);

        var specialty = await db.Specialties.Include(s => s.Faculty).FirstOrDefaultAsync(s => s.Id == group.SpecialtyId);

        groupToView.SpecialtyName = specialty.Name;
        groupToView.FacultyName = specialty.Faculty.Name;

        groupToView.CountOfStudents = await db.UserGroups.CountAsync(x => x.GroupId == id && x.Status == Domain.Models.UserGroupStatus.Member);
        return Result<GroupViewModel>.SuccessWithData(groupToView);
    }

    public async Task<Result<List<GroupViewModel>>> GetGroupsBySpecialtyIdAsync(int specialtyId)
    {
        var groups = await db.Groups
            .AsNoTracking()
            .Where(s => s.SpecialtyId == specialtyId)
            .OrderBy(s => s.Name).ThenBy(s => s.Course)
            .ToListAsync();

        var totalCount = await commonService.CountAsync<Group>(s => s.SpecialtyId == specialtyId);

        var groupsViewModels = mapper.Map<List<GroupViewModel>>(groups);

        return Result<List<GroupViewModel>>.SuccessList(groupsViewModels, Meta.FromMeta(totalCount, 0, 0));
    }

    public async Task<Result<List<GroupShortViewModel>>> GetUserGroupsAsync(int userId)
    {
        var userGroups = await db.UserGroups
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .Include(s => s.Group)
            .Select(s => s.Group)
            .OrderByDescending(s => s.Id)
            .ToListAsync();
        var groups = mapper.Map<List<GroupShortViewModel>>(userGroups);

        var totalCount = await commonService.CountAsync<UserGroup>(s => s.UserId == userId);

        return Result<List<GroupShortViewModel>>.SuccessList(groups, Meta.FromMeta(totalCount, 0, 0));
    }

    public async Task<Result<GroupViewModel>> IncreaseCourseOfGroupAsync(int groupId)
    {
        var query = await commonService.IsExistWithResultsAsync<Group>(x => x.Id == groupId);

        if (!query.IsExist)
            return Result<GroupViewModel>.NotFound(typeof(Group).NotFoundMessage(groupId));

        var group = query.Results.First();

        if (group.Course >= 6)
            return Result<GroupViewModel>.Error($"Group course can`t be more then {group.Course}");

        group.IncreaseCourse();

        group.PrepareToUpdate(identityService);
        db.Groups.Update(group);
        await db.SaveChangesAsync();

        return Result<GroupViewModel>.SuccessWithData(mapper.Map<GroupViewModel>(group));
    }

    public async Task<Result<List<GroupViewModel>>> SearchGroupsAsync(SearchGroupOptions options)
    {
        options.PrepareOptions();

        var query = db.Groups.AsNoTracking();

        if (!string.IsNullOrEmpty(options.Name))
            query = query.Where(s => s.Name.Contains(options.Name));

        if (options.Course != null)
            query = query.Where(s => s.Course == options.Course);

        if (options.SpecialtyId != null)
            query = query.Where(s => s.SpecialtyId == options.SpecialtyId);

        if (options.From != null)
            query = query.Where(s => s.StartStudy == options.From);

        if (options.To != null)
            query = query.Where(s => s.EndStudy == options.To);

        query = query.Skip(options.Offset).Take(options.Count);

        query = query.OrderBy(s => s.Name);

        var groups = await query.ToListAsync();

        var groupsToView = mapper.Map<List<GroupViewModel>>(groups);

        return Result<List<GroupViewModel>>.SuccessWithData(groupsToView);
    }

    public async Task<Result<GroupMemberViewModel>> UpdateClassTeacherGroupAsync(GroupClassTeacherEditModel model)
    {
        if (!identityService.IsAdministrator())
            return Result<GroupMemberViewModel>.Forbiden();

        var query = await commonService.IsExistWithResultsAsync<Group>(s => s.Id == model.GroupId.Value);

        if (!query.IsExist)
            return Result<GroupMemberViewModel>.NotFound(typeof(Group).NotFoundMessage(model.GroupId.Value));

        if (!await commonService.IsExistAsync<User>(s => s.Id == model.UserId))
            return Result<GroupMemberViewModel>.NotFound(typeof(User).NotFoundMessage(model.UserId));

        var currentUserGroup = await db.UserGroups
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.GroupId == model.GroupId.Value && s.IsAdmin);

        if (currentUserGroup == null)
        {

            var exist = await commonService.IsExistWithResultsAsync<UserGroupRole>(s => s.UniqId == UserGroupRoles.UniqIds.ClassTeacher);
            if (!exist.IsExist)
                return Result<GroupMemberViewModel>.NotFound(typeof(UserGroupRole).NotFoundMessage(UserGroupRoles.UniqIds.ClassTeacher));

            currentUserGroup = new UserGroup
            {
                GroupId = model.GroupId.Value,
                IsAdmin = true,
                Status = UserGroupStatus.Member,
                Title = model.Title,
                UserGroupRoleId = exist.Results.First().Id,
                UserId = model.UserId
            };

            currentUserGroup.PrepareToCreate(identityService);
            await db.UserGroups.AddAsync(currentUserGroup);
        }
        else
        {
            if (currentUserGroup.UserId == model.UserId)
                return Result<GroupMemberViewModel>.Success();
            currentUserGroup.UserId = model.UserId;
            currentUserGroup.Title = model.Title;
            currentUserGroup.PrepareToUpdate(identityService);
            db.UserGroups.Update(currentUserGroup);
        }

        await db.SaveChangesAsync();

        var groupMember = await db.UserGroups
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.UserGroupRole)
            .FirstOrDefaultAsync(s => s.Id == currentUserGroup.Id);

        var updatedGroupMember = mapper.Map<GroupMemberViewModel>(groupMember);

        return Result<GroupMemberViewModel>.SuccessWithData(updatedGroupMember);
    }
}
