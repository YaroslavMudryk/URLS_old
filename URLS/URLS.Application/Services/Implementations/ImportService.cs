using ClosedXML.Excel;
using Extensions.Password;
using Force.DeepCloner;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Export;
using URLS.Constants;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class ImportService(
    URLSDbContext db,
    IIdentityService identityService) : IImportService
{
    public async Task<Result<ExportViewModel>> ImportNewStudentsAsync(Stream stream)
    {
        using var importExcelFile = new XLWorkbook(stream);

        using var exportExcelFile = new XLWorkbook();
        exportExcelFile.Style.Font.FontSize = 14;

        var countOfGroups = importExcelFile.Worksheets.Count();

        var specialtyId = Convert.ToInt32(importExcelFile.Worksheets.ToArray()[0].Cell("A1").Value);

        var specialty = await db.Specialties.FindAsync(specialtyId);

        var userGroupRole = await db.UserGroupRoles.AsNoTracking().FirstOrDefaultAsync(s => s.UniqId == UserGroupRoles.UniqIds.Student);

        List<Group> groups = new List<Group>();

        foreach (var workSheetGroup in importExcelFile.Worksheets)
        {
            var groupName = workSheetGroup.Name;
            groupName = ValidateAndModifyIfNeeded(groupName);

            var newGroup = new Group();
            newGroup.Name = groupName;
            newGroup.Course = 1;
            newGroup.SpecialtyId = specialtyId;
            newGroup.StartStudy = DateTime.Today.GetStartStudy();
            newGroup.EndStudy = DateTime.Today.GetEndStudy();
            newGroup.IndexNumber = newGroup.Name.GetIndexForGroup();
            newGroup.PrepareToCreate(identityService);
            newGroup.UserGroups = new List<UserGroup>();

            var listOfStudents = new List<User>();

            var rows = workSheetGroup.RowsUsed().ToArray();

            for (int i = 2; i < rows.Count(); i++)
            {
                var row = rows[i];

                var newStudent = row.GetStudent();
                newStudent.ModifyUserFromImport();
                listOfStudents.Add(newStudent.DeepClone());

                newStudent.IsActivateAccount = true;
                newStudent.PasswordHash = newStudent.PasswordHash.GeneratePasswordHash();
                newStudent.FromImport = true;
                newStudent.SetLock(true);
                newStudent.PrepareToCreate(identityService);

                var newUserGroup = new UserGroup
                {
                    User = newStudent,
                    Group = newGroup,
                    IsAdmin = false,
                    Status = UserGroupStatus.Member,
                    Title = "Студент",
                    UserGroupRoleId = userGroupRole.Id
                };
                newUserGroup.PrepareToCreate(identityService);

                newGroup.UserGroups.Add(newUserGroup);
            }

            var currentWorkShit = exportExcelFile.AddWorksheet(groupName);

            currentWorkShit.SetupStudents(listOfStudents);

            var newInvite = new GroupInvite
            {
                ActiveFrom = Defaults.GroupInviteActiveFrom,
                ActiveTo = Defaults.GroupInviteActiveTo,
                CodeJoin = Generator.CreateGroupInviteCode(),
                Group = newGroup,
                IsActive = true,
                Name = "Головне запрошення"
            };

            newInvite.PrepareToCreate();

            newGroup.GroupInvites = [newInvite];
            newGroup.PrepareToCreate(identityService);
            groups.Add(newGroup);
        }

        var thisYear = DateTime.Today.GetStartStudy();

        var isExistGroups = await db.Groups.Where(s => groups.Select(c => c.Name).Contains(s.Name) && s.StartStudy == thisYear).ToListAsync();

        if (isExistGroups.Any())
        {
            return Result<ExportViewModel>.Error($"Groups with name: ({string.Join(", ", isExistGroups.Select(s => s.Name))}) already present");
        }

        await db.Groups.AddRangeAsync(groups);
        await db.SaveChangesAsync();

        var fileName = $"{specialty.Name} ({DateTime.Now.ToString("HH:mm dd-MM-yyyy")})" + ".xlsx";

        var exportModel = new ExportViewModel
        {
            FileName = fileName,
            Stream = new MemoryStream()
        };

        exportExcelFile.SaveAs(exportModel.Stream);
        exportModel.Stream.Position = 0;

        return Result<ExportViewModel>.SuccessWithData(exportModel);
    }

    private string ValidateAndModifyIfNeeded(string groupName)
    {
        if (groupName.Contains("-"))
            return groupName;

        var firstNumberIndex = groupName.IndexOfAny("0123456789".ToArray());

        return groupName.Insert(firstNumberIndex, "-");
    }
}
