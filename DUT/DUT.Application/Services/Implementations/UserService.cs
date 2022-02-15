﻿using AutoMapper;
using DUT.Application.Extensions;
using DUT.Application.Helpers;
using DUT.Application.Options;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.User;
using DUT.Constants;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Extensions.Password;
using Microsoft.EntityFrameworkCore;
namespace DUT.Application.Services.Implementations
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public UserService(DUTDbContext db, IMapper mapper, IIdentityService identityService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<UserViewModel>> CreateUserAsync(UserCreateModel model)
        {
            if (await IsExistAsync(s => s.Login == model.Login))
                return Result<UserViewModel>.Error("Login is busy");

            if (!await _db.Roles.AsNoTracking().AnyAsync(s => s.Id == model.RoleId))
                return Result<UserViewModel>.NotFound("Role not found");

            if (!string.IsNullOrEmpty(model.UserName))
                if (await IsExistAsync(s => s.UserName == model.UserName))
                    return Result<UserViewModel>.Error("Username is busy");

            var newUser = new User(model.FirstName, model.MiddleName, model.LastName, model.Login, null);
            newUser.UserName = model.UserName ?? Generator.GetUsername();
            newUser.PrepareToCreate(_identityService);
            newUser.Login = model.Login;
            newUser.PasswordHash = model.Password.GeneratePasswordHash();
            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();

            var userRole = new UserRole
            {
                UserId = newUser.Id,
                RoleId = model.RoleId
            };
            userRole.PrepareToCreate(_identityService);

            await _db.UserRoles.AddAsync(userRole);

            var notify = NotificationsHelper.GetWelcomeNotification();
            notify.UserId = newUser.Id;

            await _db.Notifications.AddAsync(notify);
            await _db.SaveChangesAsync();

            return Result<UserViewModel>.SuccessWithData(_mapper.Map<UserViewModel>(newUser)); ;
        }

        public async Task<Result<List<UserShortViewModel>>> GetLastUsersAsync(int count)
        {
            var lastUsers = await _db.Users
                .AsNoTracking()
                .OrderByDescending(s => s.JoinAt)
                .Take(count)
                .ToListAsync();
            return Result<List<UserShortViewModel>>.SuccessWithData(_mapper.Map<List<UserShortViewModel>>(lastUsers));
        }

        public async Task<Result<List<UserShortViewModel>>> GetTeachersAsync(int offset = 0, int count = 20)
        {
            var teachers = await _db.UserRoles
                .Where(s => s.RoleId == 1)
                .Include(s => s.User)
                .OrderBy(s => s.UserId)
                .Skip(offset).Take(count)
                .Select(s => new UserShortViewModel
                {
                    Id = s.User.Id,
                    FirstName = s.User.FirstName,
                    LastName = s.User.LastName,
                    UserName = s.User.UserName,
                    Image = s.User.Image,
                    JoinAt = s.User.JoinAt
                })
                .ToListAsync();
            return Result<List<UserShortViewModel>>.SuccessWithData(teachers);
        }

        public async Task<Result<UserViewModel>> GetUserByIdAsync(int id)
        {
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return Result<UserViewModel>.NotFound("User not found");
            return Result<UserViewModel>.SuccessWithData(_mapper.Map<UserViewModel>(user));
        }

        public async Task<Result<List<UserShortViewModel>>> SearchUsersAsync(SearchUserOptions searchUserOptions)
        {
            searchUserOptions.PrepareOptions();

            IQueryable<User> query = _db.Users;

            query = query.AsNoTracking();

            query = query.Skip(searchUserOptions.Offset).Take(searchUserOptions.Count);

            if (!string.IsNullOrEmpty(searchUserOptions.FirstName))
                query = query.Where(x => x.FirstName.Contains(searchUserOptions.FirstName));

            if (!string.IsNullOrEmpty(searchUserOptions.LastName))
                query = query.Where(x => x.LastName.Contains(searchUserOptions.LastName));

            //Other filters


            var result = await query.OrderBy(x => x.Id).ToListAsync();

            return Result<List<UserShortViewModel>>.SuccessWithData(_mapper.Map<List<UserShortViewModel>>(result));
        }

        public async Task<Result<UserViewModel>> UpdateUsernameAsync(UsernameUpdateModel model)
        {
            if (model.UserId != _identityService.GetUserId())
                if (!_identityService.GetRoles().Contains(Roles.Admin))
                    return Result<UserViewModel>.Error("Access denited");

            var userToUpdate = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.UserId);
            if (userToUpdate == null)
                return Result<UserViewModel>.NotFound("User by id not found");

            if (userToUpdate.UserName == model.Username)
                return Result<UserViewModel>.Error("Username equals current you");

            if (await IsExistAsync(s => s.UserName == model.Username))
                return Result<UserViewModel>.Error("Username is already busy");

            userToUpdate.UserName = model.Username;
            userToUpdate.PrepareToUpdate(_identityService);
            _db.Users.Update(userToUpdate);
            await _db.SaveChangesAsync();
            return Result<UserViewModel>.SuccessWithData(_mapper.Map<UserViewModel>(userToUpdate));
        }
    }
}