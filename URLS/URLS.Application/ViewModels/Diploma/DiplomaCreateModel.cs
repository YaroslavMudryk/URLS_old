using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Diploma;

public class DiplomaCreateModel
{
    [Required, StringLength(5)]
    public string Series { get; set; }
    [Required]
    public int Number { get; set; }
}
