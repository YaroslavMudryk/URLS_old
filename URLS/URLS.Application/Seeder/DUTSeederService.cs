using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Identity;
using URLS.Constants;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Seeder;

public class DUTSeederService(
    URLSDbContext db,
    IAuthenticationService authenticationService,
    IConfiguration configuration) : BaseSeederService(db, authenticationService, configuration)
{
    public override async Task<Result<JwtToken>> SeedSystemAsync()
    {
        #region University

        int universityId = 0;

        if (!await _db.Universities.AnyAsync())
        {
            var newUniversity = new University
            {
                Name = "Державний університет телекомунікацій",
                NameEng = "State University of Telecommunications",
                ShortName = "ДУТ",
                ShortNameEng = "SUT"
            };
            newUniversity.PrepareToCreate();

            await _db.Universities.AddAsync(newUniversity);
            await _db.SaveChangesAsync();
            universityId = newUniversity.Id;
            _countOfInitEntities++;
        }

        #endregion

        #region Faculties

        List<Faculty> facuties = [];

        if (!await _db.Faculties.AnyAsync())
        {
            var listFaculties = new List<Faculty>
            {
                new Faculty
                {
                    Name = "Навчально-науковий інститут захисту інформації"
                },
                new Faculty
                {
                    Name = "Навчально-Науковий Інститут Інформаційних Технологій"
                },
                new Faculty
                {
                    Name = "Навчально-науковий інститут Телекомунікацій"
                },
                new Faculty
                {
                    Name = "Навчально-науковий інститут менеджменту та підприємництва"
                },
                new Faculty
                {
                    Name = "Навчально-науковий інститут заочного та дистанційного навчання"
                },
                new Faculty
                {
                    Name = "Навчально-науковий інститут гуманітарних та природничих дисциплін"
                },
                new Faculty
                {
                    Name = "Аспірантура"
                }
            };

            listFaculties.ForEach(x =>
            {
                x.UniversityId = universityId;
                x.PrepareToCreate();
            });

            await _db.Faculties.AddRangeAsync(listFaculties.ToArray());
            await _db.SaveChangesAsync();
            facuties.AddRange(listFaculties);
            _countOfInitEntities++;
        }

        #endregion

        #region Specialties

        if (!await _db.Specialties.AnyAsync())
        {
            var listSpecialties = new List<Specialty>
            {
                new Specialty
                {
                    Name = "Інженерія програмного забезпечення",
                    Code = "121",
                    Invite = Generator.CreateGroupInviteCode()
                },
                new Specialty
                {
                    Name = "Комп'ютерні науки",
                    Code = "122",
                    Invite = Generator.CreateGroupInviteCode()
                },
                new Specialty
                {
                    Name = "Комп'ютерна інженерія",
                    Code = "123",
                    Invite = Generator.CreateGroupInviteCode()
                }
            };

            var facultyInfoId = GetFacultyIdByName("Навчально-Науковий Інститут Інформаційних Технологій");

            listSpecialties.ForEach(x =>
            {
                x.PrepareToCreate();
                x.FacultyId = facultyInfoId;
            });

            await _db.Specialties.AddRangeAsync(listSpecialties.ToArray());
            await _db.SaveChangesAsync();
            _countOfInitEntities++;
        }

        #endregion

        int GetFacultyIdByName(string name)
        {
            return facuties.FirstOrDefault(x => x.Name == name).Id;
        }

        return await base.SeedSystemAsync();
    }
}