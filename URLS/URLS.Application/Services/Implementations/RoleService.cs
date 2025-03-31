using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Helpers;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.RoleClaim;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class RoleService(
    URLSDbContext db,
    IMapper mapper,
    IIdentityService identityService,
    IClaimService claimService,
    INotificationService notificationService,
    ICommonService commonService) : IRoleService
{
    public async Task<Result<RoleViewModel>> CreateRoleAsync(RoleCreateModel model)
    {
        if (await commonService.IsExistAsync<Role>(s => s.Name == model.Name && s.ClaimsHash == model.ClaimsIds.GetHashForClaimIds()))
        {
            return Result<RoleViewModel>.Error("Same role already exist");
        }

        if (!await CheckClaimsAsync(model.ClaimsIds))
        {
            return Result<RoleViewModel>.Error("Not all claims exist");
        }

        var newRole = new Role
        {
            Name = model.Name,
            CanDelete = true,
            CountClaims = model.ClaimsIds.Count(),
            ClaimsHash = model.ClaimsIds.GetHashForClaimIds()
        };
        newRole.PrepareToCreate(identityService);

        await db.Roles.AddAsync(newRole);
        await db.SaveChangesAsync();

        var roleClaims = model.ClaimsIds.Select(s => new RoleClaim
        {
            RoleId = newRole.Id,
            ClaimId = s
        }).ToList();

        roleClaims.ForEach(x =>
        {
            x.PrepareToCreate(identityService);
        });

        await db.RoleClaims.AddRangeAsync(roleClaims);
        await db.SaveChangesAsync();

        return Result<RoleViewModel>.Created(mapper.Map<RoleViewModel>(newRole));
    }

    public async Task<Result<List<RoleViewModel>>> GetAllRolesAsync()
    {
        var roles = await db.Roles.AsNoTracking().ToListAsync();

        var rolesToView = mapper.Map<List<RoleViewModel>>(roles);

        return Result<List<RoleViewModel>>.SuccessList(rolesToView);
    }

    public async Task<Result<RoleViewModel>> GetRoleByIdAsync(int id, bool withClaims)
    {
        var query = await commonService.IsExistWithResultsAsync<Role>(s => s.Id == id);
        if (!query.IsExist)
            return Result<RoleViewModel>.NotFound("Role not found");

        var role = query.Results.First();

        var roleToView = mapper.Map<RoleViewModel>(role);

        if (withClaims)
        {
            var res = await claimService.GetClaimsByRoleIdAsync(role.Id);
            if (res.IsSuccess)
                roleToView.Claims = res.Data;
        }
        return Result<RoleViewModel>.SuccessWithData(roleToView);
    }

    public async Task<Result<RoleViewModel>> RemoveRoleAsync(int roleId)
    {
        var query = await commonService.IsExistWithResultsAsync<Role>(s => s.Id == roleId);
        if (!query.IsExist)
            return Result<RoleViewModel>.NotFound("Role not found");

        var roleToRemove = query.Results.First();

        if (!roleToRemove.CanDelete)
            return Result<RoleViewModel>.Error("This role can`t remove");

        var roleClaims = await db.RoleClaims.Where(s => s.RoleId == roleId).ToListAsync();
        if (roleClaims.Any())
        {
            db.RoleClaims.RemoveRange(roleClaims);
            await db.SaveChangesAsync();
        }
        db.Roles.Remove(roleToRemove);
        await db.SaveChangesAsync();
        return Result<RoleViewModel>.Success();
    }

    public async Task<Result<RoleViewModel>> UpdateRoleAsync(RoleEditModel model)
    {
        var query = await commonService.IsExistWithResultsAsync<Role>(s => s.Id == model.Id);
        if (!query.IsExist)
            return Result<RoleViewModel>.NotFound("Role not found");

        var roleToUpdate = query.Results.First();

        if (!CheckIsNeedToUpdateRole(roleToUpdate, model))
            return Result<RoleViewModel>.SuccessWithData(mapper.Map<RoleViewModel>(roleToUpdate));

        var isAllClaimsExist = await CheckClaimsAsync(model.ClaimsIds);
        if (!isAllClaimsExist)
        {
            return Result<RoleViewModel>.Error("Not all claims exist");
        }

        var roleClaims = await db.RoleClaims.AsNoTracking().Where(s => s.RoleId == model.Id).ToListAsync();

        db.RoleClaims.RemoveRange(roleClaims);
        await db.SaveChangesAsync();

        var newRoleClaims = model.ClaimsIds.Select(x => new RoleClaim
        {
            ClaimId = x,
            RoleId = model.Id,
        }).ToList();

        newRoleClaims.ForEach(s =>
        {
            s.PrepareToCreate(identityService);
        });

        roleToUpdate.CountClaims = newRoleClaims.Count;
        roleToUpdate.ClaimsHash = model.ClaimsIds.GetHashForClaimIds();

        await db.RoleClaims.AddRangeAsync(newRoleClaims);
        await db.SaveChangesAsync();

        var userIds = await db.UserRoles.AsNoTracking().Where(s => s.RoleId == model.Id).Select(s => s.UserId).ToListAsync();

        var res = await notificationService.SendNotifyToUsersAsync(NotificationsHelper.GetChangePermissionNotification(), userIds);

        return Result<RoleViewModel>.SuccessWithData(mapper.Map<RoleViewModel>(roleToUpdate));
    }

    private async Task<bool> CheckClaimsAsync(int[] claimsIds)
    {
        var claims = await db.Claims.AsNoTracking().Where(s => claimsIds.Contains(s.Id)).ToListAsync();
        return claims.Count == claimsIds.Count();
    }

    private bool CheckIsNeedToUpdateRole(Role currentRole, RoleEditModel updatedRole)
    {
        var countOfNewRoleClaims = updatedRole.ClaimsIds.Count();
        var hash = updatedRole.ClaimsIds.GetHashForClaimIds();
        var name = updatedRole.Name;
        return countOfNewRoleClaims != currentRole.CountClaims
            || hash != currentRole.ClaimsHash
            || currentRole.Name != name;
    }
}
