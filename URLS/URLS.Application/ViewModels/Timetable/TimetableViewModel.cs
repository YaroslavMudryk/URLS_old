using URLS.Application.ViewModels.Subject;
using URLS.Application.ViewModels.User;
using URLS.Domain.Models;

namespace URLS.Application.ViewModels.Timetable;

public class TimetableViewModel
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserViewModel Teacher { get; set; }
    public SubjectViewModel Subject { get; set; }
    public bool IsHoliday { get; set; }
    public Holiday Holiday { get; set; }
    public LessonType Type { get; set; }
    public LessonTime Time { get; set; }
    public DateTime Date { get; set; }
}
