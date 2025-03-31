using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Group;

public class GroupCreateModel
{
    [Required, StringLength(50, MinimumLength = 1)]
    public string Name { get; set; }
    [StringLength(1000, MinimumLength = 1)]
    public string Image { get; set; }
    [Required]
    public DateTime StartStudy { get; set; }
    [Required]
    public DateTime EndStudy { get; set; }
    [Required]
    public int Course { get; set; }
    [Required]
    public int SpecialtyId { get; set; }
    public int? ClassTeacherId { get; set; }
}
