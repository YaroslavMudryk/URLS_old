﻿using AutoMapper;
using DUT.Application.Extensions;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.University;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class UniversityService : BaseService<University>, IUniversityService
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly DUTDbContext _db;
        public UniversityService(IMapper mapper, DUTDbContext db, IIdentityService identityService) : base(db)
        {
            _mapper = mapper;
            _db = db;
            _identityService = identityService;
        }

        public async Task<Result<UniversityViewModel>> CreateUniversityAsync(UniversityCreateModel model)
        {
            var count = await CountAsync();
            if (count > 0)
                return Result<UniversityViewModel>.Error("University is already exist");

            var newUniversity = new University
            {
                Name = model.Name,
                ShortName = model.ShortName,
                NameEng = model.NameEng,
                ShortNameEng = model.ShortNameEng
            };
            newUniversity.PrepareToCreate(_identityService);

            await _db.Universities.AddAsync(newUniversity);
            await _db.SaveChangesAsync();
            return Result<UniversityViewModel>.SuccessWithData(_mapper.Map<UniversityViewModel>(newUniversity));
        }

        public async Task<Result<UniversityViewModel>> GetUniversityAsync()
        {
            var university = await _db.Universities.AsNoTracking().FirstOrDefaultAsync();
            if (university == null)
                return Result<UniversityViewModel>.NotFound("University not found");
            var universityViewModel = _mapper.Map<UniversityViewModel>(university);
            return Result<UniversityViewModel>.SuccessWithData(universityViewModel);
        }

        public async Task<Result<UniversityViewModel>> UpdateUniversityAsync(UniversityEditModel model)
        {
            var updatedUniversity = await _db.Universities.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.Id);
            if (updatedUniversity == null)
                return Result<UniversityViewModel>.NotFound();

            updatedUniversity.Name = model.Name;
            updatedUniversity.ShortName = model.ShortName;
            updatedUniversity.NameEng = model.NameEng;
            updatedUniversity.ShortNameEng = model.ShortNameEng;
            updatedUniversity.PrepareToUpdate(_identityService);

            _db.Universities.Update(updatedUniversity);
            await _db.SaveChangesAsync();
            return Result<UniversityViewModel>.SuccessWithData(_mapper.Map<UniversityViewModel>(updatedUniversity));
        }
    }
}