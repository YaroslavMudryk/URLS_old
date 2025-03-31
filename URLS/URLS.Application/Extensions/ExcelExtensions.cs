using ClosedXML.Excel;
using Extensions.Password;
using URLS.Constants;
using URLS.Domain.Models;

namespace URLS.Application.Extensions;

public static class ExcelExtensions
{
    public static User GetStudent(this IXLRow row)
    {
        User user = new User(
            row.Cell(2).GetValue<string>(),
            row.Cell(3).GetValue<string>(),
            row.Cell(1).GetValue<string>(),
            row.Cell(5).GetValue<string>(),
            Generator.GetUsername())
        {
            ContactPhone = row.Cell(6).GetValue<string>(),
            PasswordHash = "adsafedgs".GeneratePasswordHash()
        };
        return user;
    }

    public static T GetValue<T>(this IXLCell cell)
    {
        return (T)Convert.ChangeType(cell.Value, typeof(T));
    }

    public static void SetupStudents(this IXLWorksheet worksheet, List<User> users)
    {
        worksheet.Cell(1, 1).Value = "ID";
        worksheet.Cell(1, 2).Value = "Студент";
        worksheet.Cell(1, 3).Value = "Логін";
        worksheet.Cell(1, 4).Value = "Пароль";

        var row = 2;
        foreach (var student in users.OrderBy(s => s.LastName))
        {
            worksheet.Cell(row, 1).Value = student.Id;
            worksheet.Cell(row, 2).Value = $"{student.LastName} {student.FirstName}";
            worksheet.Cell(row, 3).Value = student.Login;
            worksheet.Cell(row, 4).Value = student.PasswordHash;
            row++;
        }
    }
}
