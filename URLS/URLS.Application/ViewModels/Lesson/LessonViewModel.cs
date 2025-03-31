using URLS.Application.ViewModels.Subject;
using URLS.Application.ViewModels.User;
using URLS.Domain.Models;

namespace URLS.Application.ViewModels.Lesson;

public class LessonViewModel
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Theme { get; set; }
    public string Description { get; set; }
    public Journal Journal { get; set; }
    public DateTime Date { get; set; }
    public LessonType LessonType { get; set; }
    public string Homework { get; set; }
    public bool IsSubstitute { get; set; }
    public UserViewModel SubstituteTeacher { get; set; }
    public LessonViewModel PreviewLesson { get; set; }
    public LessonViewModel NextLesson { get; set; }
    public SubjectViewModel Subject { get; set; }
}
