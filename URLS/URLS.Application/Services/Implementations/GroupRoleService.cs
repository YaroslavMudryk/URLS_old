using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group.GroupRole;
using URLS.Constants.APIResponse;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class GroupRoleService(
    URLSDbContext db,
    IMapper mapper,
    IIdentityService identityService,
    ICommonService commonService) : IGroupRoleService
{
    public async Task<Result<UserGroupRoleViewModel>> CreateGroupRoleAsync(UserGroupRoleCreateModel model)
    {
        if (await commonService.IsExistAsync<UserGroupRole>(s => s.Name == model.Name))
        {
            return Result<UserGroupRoleViewModel>.Error("This role already exist");
        }

        var newUserGroupRole = new UserGroupRole
        {
            Name = model.Name,
            NameEng = model.NameEng,
            CanEdit = true,
            Color = model.Color,
            Description = model.Description,
            DescriptionEng = model.DescriptionEng,
            Permissions = model.Permissions,
            UniqId = Guid.NewGuid().ToString().ToUpper()
        };
        newUserGroupRole.PrepareToCreate(identityService);
        await db.UserGroupRoles.AddAsync(newUserGroupRole);
        await db.SaveChangesAsync();

        return Result<UserGroupRoleViewModel>.Created(mapper.Map<UserGroupRoleViewModel>(newUserGroupRole));
    }

    public async Task<Result<List<UserGroupRoleViewModel>>> GetAllGroupRolesAsync()
    {
        var allGroupRoles = await db.UserGroupRoles.AsNoTracking().ToListAsync();

        var groupRolesViewModels = mapper.Map<List<UserGroupRoleViewModel>>(allGroupRoles);

        return Result<List<UserGroupRoleViewModel>>.SuccessList(groupRolesViewModels, Meta.FromMeta(allGroupRoles.Count, 0, 0));
    }

    public async Task<Result<UserGroupRoleViewModel>> GetGroupRoleByIdAsync(int id)
    {
        var userGroupRole = await db.UserGroupRoles.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        if (userGroupRole == null)
            return Result<UserGroupRoleViewModel>.NotFound(typeof(UserGroupRole).NotFoundMessage(id));
        return Result<UserGroupRoleViewModel>.SuccessWithData(mapper.Map<UserGroupRoleViewModel>(userGroupRole));
    }

    public async Task<Result<bool>> RemoveGroupRoleAsync(int id)
    {
        var userGroupRoleToRemove = await db.UserGroupRoles.FindAsync(id);
        if (userGroupRoleToRemove == null)
            return Result<bool>.NotFound(typeof(UserGroupRole).NotFoundMessage(id));

        if (!userGroupRoleToRemove.CanEdit)
            return Result<bool>.Error("This role can't be remove");

        db.UserGroupRoles.Remove(userGroupRoleToRemove);
        await db.SaveChangesAsync();

        return Result<bool>.Success();
    }

    public async Task<Result<UserGroupRoleViewModel>> UpdateGroupRoleAsync(UserGroupRoleEditModel model)
    {
        var userGroupRoleToUpdate = await db.UserGroupRoles.FindAsync(model.Id);
        if (userGroupRoleToUpdate == null)
            return Result<UserGroupRoleViewModel>.NotFound(typeof(UserGroupRole).NotFoundMessage(model.Id));

        if(!userGroupRoleToUpdate.CanEdit)
            return Result<UserGroupRoleViewModel>.Error("This role can't be edit");

        userGroupRoleToUpdate.Name = model.Name;
        userGroupRoleToUpdate.NameEng = model.NameEng;
        userGroupRoleToUpdate.Description = model.Description;
        userGroupRoleToUpdate.DescriptionEng = model.DescriptionEng;
        userGroupRoleToUpdate.Color = model.Color;
        userGroupRoleToUpdate.Permissions = model.Permissions;
        userGroupRoleToUpdate.PrepareToUpdate(identityService);


        db.UserGroupRoles.Update(userGroupRoleToUpdate);
        await db.SaveChangesAsync();
        return Result<UserGroupRoleViewModel>.SuccessWithData(mapper.Map<UserGroupRoleViewModel>(userGroupRoleToUpdate));
    }
}
