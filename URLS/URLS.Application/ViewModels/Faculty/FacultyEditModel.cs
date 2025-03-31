using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Faculty;

public class FacultyEditModel : FacultyCreateModel
{
    [Required]
    public int Id { get; set; }

    public FacultyEditModel()
    {

    }

    public FacultyEditModel(FacultyViewModel model) : base(model)
    {
        Id = model.Id;
    }
}
