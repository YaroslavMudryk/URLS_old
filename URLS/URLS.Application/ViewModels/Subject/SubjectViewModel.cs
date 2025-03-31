using URLS.Application.ViewModels.Group;
using URLS.Application.ViewModels.Lesson;
using URLS.Application.ViewModels.User;
using URLS.Domain.Models;

namespace URLS.Application.ViewModels.Subject;

public class SubjectViewModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Semestr { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public bool IsTemplate { get; set; }
    public SubjectConfig Config { get; set; }
    public GroupViewModel Group { get; set; }
    public UserViewModel Teacher { get; set; }
    public List<LessonViewModel> Lessons { get; set; }
}
