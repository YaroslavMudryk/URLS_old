using URLS.Application.ViewModels.Specialty;

namespace URLS.Application.ViewModels.Group;

public class GroupViewModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public int Course { get; set; }
    public DateTime StartStudy { get; set; }
    public DateTime EndStudy { get; set; }
    public bool StudyingIsOver { get; set; }
    public int CountOfStudents { get; set; }
    public string SpecialtyName { get; set; }
    public string FacultyName { get; set; }
    public SpecialtyViewModel Specialty { get; set; }
    public List<GroupInviteViewModel> GroupInvites { get; set; }
}
