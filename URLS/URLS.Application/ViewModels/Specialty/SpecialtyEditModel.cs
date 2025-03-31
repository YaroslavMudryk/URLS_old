using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Specialty;

public class SpecialtyEditModel : SpecialtyCreateModel
{
    [Required]
    public int Id { get; set; }

    public SpecialtyEditModel()
    {

    }

    public SpecialtyEditModel(SpecialtyViewModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Code = model.Code;
    }
}
