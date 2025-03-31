using URLS.Application.ViewModels.User;

namespace URLS.Application.ViewModels.Specialty;

public class SpecialtyTeacherViewModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; }
    public UserViewModel Teacher { get; set; }
}
