using ClosedXML.Excel;
using URLS.Application.Extensions;
using URLS.Application.ViewModels.Export;
using URLS.Domain.Models;

namespace URLS.Application.Helpers;

public static class ExportHelper
{
    private static string _extension = ".xlsx";
    public static ExportViewModel ExportGroupFile(Group group, List<UserGroup> userGroups)
    {
        using var workbook = new XLWorkbook();
        workbook.Style.Font.FontSize = 14;

        var workShitGroup = workbook.AddWorksheet("Група");

        workShitGroup.Cell(1, 1).Value = "Назва:";
        workShitGroup.Cell(1, 2).Value = group.Name;
        workShitGroup.Cell(2, 1).Value = "Курс:";
        workShitGroup.Cell(2, 2).Value = $"{group.Course} курс";
        workShitGroup.Cell(3, 1).Value = "Початок навчання:";
        workShitGroup.Cell(3, 2).Value = group.StartStudy;
        workShitGroup.Cell(4, 1).Value = "Кінець навчання:";
        workShitGroup.Cell(4, 2).Value = group.EndStudy.ToString("dd.MM.yyyy р.");

        workShitGroup.Cell(6, 1).Value = "ПІБ куратора:";
        var admin = userGroups.FirstOrDefault(s => s.IsAdmin);
        workShitGroup.Cell(6, 2).Value = $"{admin.User.LastName} {admin.User.FirstName}";
        workShitGroup.Cell(7, 1).Value = "Куратор від:";
        workShitGroup.Cell(7, 2).Value = admin.CreatedAt.ToString("HH:mm dd.MM.yyyy");



        var workShitMembers = workbook.AddWorksheet("Студенти");

        workShitMembers.Cell(1, 1).Value = "ID";
        workShitMembers.Cell(1, 2).Value = "Ім'я";
        workShitMembers.Cell(1, 3).Value = "Прізвище";
        workShitMembers.Cell(1, 4).Value = "Підпис";
        workShitMembers.Cell(1, 5).Value = "Роль";
        workShitMembers.Cell(1, 6).Value = "Статус";

        var row = 2;
        foreach (var member in userGroups.OrderBy(s => s.User.LastName).Where(s => !s.IsAdmin))
        {
            workShitMembers.Cell(row, 1).Value = member.User.Id;
            workShitMembers.Cell(row, 2).Value = member.User.FirstName;
            workShitMembers.Cell(row, 3).Value = member.User.LastName;
            workShitMembers.Cell(row, 4).Value = member.Title;
            workShitMembers.Cell(row, 5).Value = member.UserGroupRole.Name;
            workShitMembers.Cell(row, 6).Value = member.Status.GetStatusName();
            row++;
        }


        var fileName = $"Група - {group.Name.ToUpper()} ({DateTime.Now.ToString("HH:mm dd-MM-yyyy")})" + _extension;

        var exportModel = new ExportViewModel
        {
            FileName = fileName,
            Stream = new MemoryStream()
        };

        workbook.SaveAs(exportModel.Stream);
        exportModel.Stream.Position = 0;

        return exportModel;
    }

    public static ExportViewModel ExportLessonMarkFile(Lesson lesson)
    {
        using var workbook = new XLWorkbook();
        workbook.Style.Font.FontSize = 14;

        var marksWorksheet = workbook.AddWorksheet("Оцінки");

        marksWorksheet.Cell(1, 1).Value = "Студент";
        marksWorksheet.Cell(1, 2).Value = $"{lesson.Date.ToString("dd.MM.yyyy")} ({lesson.LessonType.GetLessonType()})";

        var row = 2;

        foreach (var student in lesson.Journal.Students)
        {
            marksWorksheet.Cell(row, 1).Value = student.Name;
            marksWorksheet.Cell(row, 2).Value = student.Mark;
            row++;
        }

        var fileName = $"({lesson.Subject.Group.Name}) {lesson.Subject.Name} ({lesson.Date.ToString("dd.MM.yyyy")}) ({DateTime.Now.ToString("HH:mm dd-MM-yyyy")})" + _extension;

        var exportModel = new ExportViewModel
        {
            FileName = fileName,
            Stream = new MemoryStream()
        };

        workbook.SaveAs(exportModel.Stream);
        exportModel.Stream.Position = 0;

        return exportModel;
    }

    public static ExportViewModel ExportMarksFile(Subject subject, DateTime from, DateTime to)
    {
        using var workbook = new XLWorkbook();
        workbook.Style.Font.FontSize = 14;

        var marksWorksheet = workbook.AddWorksheet("Оцінки");
        marksWorksheet.Cell(1, 1).Value = "Студент";
        marksWorksheet.ColumnWidth = 25;

        SetupDatesAndNames(marksWorksheet, subject);

        SetupMarks(marksWorksheet, subject);

        var countOfLessons = subject.Lessons.Count;

        var fileName = $"({subject.Group.Name}) {subject.Name} ({DateTime.Now.ToString("HH:mm dd-MM-yyyy")})" + _extension;

        var exportModel = new ExportViewModel
        {
            FileName = fileName,
            Stream = new MemoryStream()
        };

        workbook.SaveAs(exportModel.Stream);
        exportModel.Stream.Position = 0;

        return exportModel;
    }

    private static void SetupDatesAndNames(IXLWorksheet worksheet, Subject subject)
    {
        var column = 2;
        var row = 2;

        foreach (var lesson in subject.Lessons)
        {
            worksheet.Cell(1, column++).Value = $"{lesson.Date.ToString("dd.MM.yyyy")} ({lesson.LessonType.GetLessonType()})";
        }

        foreach (var studentName in GetAllStudentsName(subject))
        {
            worksheet.Cell(row++, 1).Value = studentName;
        }
    }

    private static void SetupMarks(IXLWorksheet worksheet, Subject subject)
    {
        var row = 2;
        var column = 2;

        foreach (var lesson in subject.Lessons)
        {
            if (lesson.Journal != null)
                foreach (var student in lesson.Journal.Students.OrderBy(s => s.Name))
                {
                    worksheet.Cell(row++, column).Value = student.Mark;
                }
            row = 2;
            column++;
        }
    }

    private static List<string> GetAllStudentsName(Subject subject)
    {
        List<string> students = new List<string>();

        subject.Lessons.ForEach(l =>
        {
            l.Journal?.Students.ForEach(s =>
            {
                if (!students.Contains(s.Name))
                    students.Add(s.Name);
            });
        });

        students.Distinct();

        students = students.OrderBy(s => s).ToList();
        return students;
    }
}
