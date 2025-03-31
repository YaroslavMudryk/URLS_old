using URLS.Application.ViewModels.Faculty;
using URLS.Application.ViewModels.Group;

namespace URLS.Application.ViewModels.Specialty;

public class SpecialtyViewModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public FacultyViewModel Faculty { get; set; }
    public List<GroupViewModel> Groups { get; set; }
}
