using System.ComponentModel.DataAnnotations;

namespace URLS.Domain.Models;

public class Specialty : BaseModel<int>
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; }
    [StringLength(10, MinimumLength = 1)]
    public string Code { get; set; }
    public string Invite { get; set; }
    public List<Group> Groups { get; set; }
    public int FacultyId { get; set; }
    public Faculty Faculty { get; set; }
}
