using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Specialty;

public class SpecialtyCreateModel
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; }
    [StringLength(10, MinimumLength = 1)]
    public string Code { get; set; }
    public int FacultyId { get; set; }
}
