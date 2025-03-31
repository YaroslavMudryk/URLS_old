using System.ComponentModel.DataAnnotations;

namespace URLS.Domain.Models;

public class Faculty : BaseModel<int>
{
    [Required, StringLength(150, MinimumLength = 1)]
    public string Name { get; set; }
    public List<Specialty> Specialties { get; set; }
    public int UniversityId { get; set; }
    public University University { get; set; }
}
