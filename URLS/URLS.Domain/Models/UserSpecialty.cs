using System.ComponentModel.DataAnnotations;

namespace URLS.Domain.Models;

public class UserSpecialty : BaseModel<int>
{
    [Required, StringLength(150, MinimumLength = 1)]
    public string Title { get; set; }
    public int SpecialtyId { get; set; }
    public Specialty Specialty { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}
