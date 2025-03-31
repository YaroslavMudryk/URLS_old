using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Specialty;

public class SpecialtyTeacherCreateModel
{
    [Required, StringLength(150, MinimumLength = 1)]
    public string Title { get; set; }
    [Required]
    public int TeacherId { get; set; }
    [Required]
    public int SpecialtyId { get; set; }
}
