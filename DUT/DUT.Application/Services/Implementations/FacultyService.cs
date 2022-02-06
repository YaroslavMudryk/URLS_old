﻿using AutoMapper;
using DUT.Application.Extensions;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Faculty;
using DUT.Application.ViewModels.Specialty;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class FacultyService : BaseService<Faculty>, IFacultyService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public FacultyService(DUTDbContext db, IMapper mapper, IIdentityService identityService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<FacultyViewModel>> CreateFacultyAsync(FacultyCreateModel model)
        {
            if (await IsExistAsync(x => x.Name == model.Name))
                return Result<FacultyViewModel>.Error("Faculty already exist");
            var faculty = new Faculty
            {
                Name = model.Name,
                UniversityId = 1
            };
            faculty.PrepareToCreate(_identityService);
            await _db.Faculties.AddAsync(faculty);
            await _db.SaveChangesAsync();
            return Result<FacultyViewModel>.SuccessWithData(_mapper.Map<FacultyViewModel>(faculty));
        }

        public async Task<Result<FacultyViewModel>> UpdateFacultyAsync(FacultyEditModel model)
        {
            var currentFaculty = await _db.Faculties.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.Id);
            if (currentFaculty == null)
                return Result<FacultyViewModel>.NotFound();
            currentFaculty.Name = model.Name;
            currentFaculty.PrepareToUpdate(_identityService);
            _db.Faculties.Update(currentFaculty);
            await _db.SaveChangesAsync();
            return Result<FacultyViewModel>.SuccessWithData(_mapper.Map<FacultyViewModel>(currentFaculty));
        }

        public async Task<Result<List<FacultyViewModel>>> GetAllFacultiesAsync()
        {
            return Result<List<FacultyViewModel>>.SuccessWithData(await _db.Faculties.AsNoTracking().Select(x => new FacultyViewModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Name = x.Name
            }).ToListAsync());
        }

        public async Task<Result<FacultyViewModel>> GetFacultyByIdAsync(int id)
        {
            var faculty = await _db.Faculties.AsNoTracking().Select(x => new FacultyViewModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Name = x.Name
            }).FirstOrDefaultAsync(x => x.Id == id);
            if (faculty == null)
                return Result<FacultyViewModel>.NotFound();
            return Result<FacultyViewModel>.SuccessWithData(faculty);
        }

        public async Task<Result<List<SpecialtyViewModel>>> GetSpecialtiesByFacultyIdAsync(int facultyId)
        {
            return Result<List<SpecialtyViewModel>>.SuccessWithData(await _db.Specialties.AsNoTracking().Where(x => x.FacultyId == facultyId).Select(x => new SpecialtyViewModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Name = x.Name,
                Code = x.Code
            }).ToListAsync());
        }
    }
}