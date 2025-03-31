using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Diploma;

public class DiplomaTemplateCreateModel
{
    [Required, StringLength(250, MinimumLength = 5)]
    public string Name { get; set; }
    [Required]
    public string Series { get; set; }
    [Required]
    public int Number { get; set; }
    [Required]
    public string Student { get; set; }
    [Required]
    public DateTime EndedAt { get; set; }
    [Required]
    public string University { get; set; }
    [Required]
    public string Specialty { get; set; }
    [Required]
    public string Qualification { get; set; }
    public string UniversityStampPath { get; set; }
    public string DirectorSignaturePath { get; set; }
}
