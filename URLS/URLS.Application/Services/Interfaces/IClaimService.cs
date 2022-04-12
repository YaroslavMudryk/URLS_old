﻿using URLS.Application.ViewModels;
using URLS.Application.ViewModels.RoleClaim;
using URLS.Domain.Models;

namespace URLS.Application.Services.Interfaces
{
    public interface IClaimService : IBaseService<Claim>
    {
        Task<Result<List<ClaimViewModel>>> GetAllClaimsAsync(int offset = 0, int limit = 100);
        Task<Result<List<ClaimViewModel>>> GetClaimsByRoleIdAsync(int roleId);
        Task<Result<ClaimViewModel>> UpdateClaimAsync(ClaimEditModel model);
    }
}