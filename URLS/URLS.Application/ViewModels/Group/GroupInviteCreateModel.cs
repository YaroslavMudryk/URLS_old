using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Group;

public class GroupInviteCreateModel
{
    [Required, StringLength(75, MinimumLength = 1)]
    public string Name { get; set; }
    [Required]
    public DateTime ActiveFrom { get; set; }
    [Required]
    public DateTime ActiveTo { get; set; }
    [Required]
    public bool IsActive { get; set; }
    public int? GroupId { get; set; }
}
