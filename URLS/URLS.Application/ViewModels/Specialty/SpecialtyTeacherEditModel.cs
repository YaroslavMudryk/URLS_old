using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Specialty;

public class SpecialtyTeacherEditModel : SpecialtyTeacherCreateModel
{
    [Required]
    public int Id { get; set; }
}
