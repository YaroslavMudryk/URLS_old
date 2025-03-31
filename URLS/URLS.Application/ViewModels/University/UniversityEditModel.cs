using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.University;

public class UniversityEditModel : UniversityCreateModel
{
    [Required]
    public int Id { get; set; }

    public UniversityEditModel()
    {

    }

    public UniversityEditModel(UniversityViewModel model) : base(model)
    {
        Id = model.Id;
    }
}
