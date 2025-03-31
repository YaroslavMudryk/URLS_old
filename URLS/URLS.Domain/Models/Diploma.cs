using System.ComponentModel.DataAnnotations;

namespace URLS.Domain.Models;

public class Diploma : BaseModelWithoutIdentity<string>
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
    public string UniversityStamp { get; set; }
    public string DirectorSignature { get; set; }
    public bool IsTemplate { get; set; }

    public int? UserId { get; set; }
    public User User { get; set; }
}
