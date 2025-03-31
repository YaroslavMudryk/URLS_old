using System.ComponentModel.DataAnnotations;

namespace URLS.Domain.Models;

public class Group : BaseModel<int>
{
    [Required, StringLength(50, MinimumLength = 1)]
    public string Name { get; set; }
    public int IndexNumber { get; set; }
    [StringLength(1000, MinimumLength = 1)]
    public string Image { get; set; }
    [Required]
    public DateTime StartStudy { get; set; }
    [Required]
    public DateTime EndStudy { get; set; }
    [Required]
    public int Course { get; set; }
    public int SpecialtyId { get; set; }
    public Specialty Specialty { get; set; }
    public List<UserGroup> UserGroups { get; set; }
    public List<GroupInvite> GroupInvites { get; set; }
    public List<Post> Posts { get; set; }
    public List<Subject> Subjects { get; set; }
}
