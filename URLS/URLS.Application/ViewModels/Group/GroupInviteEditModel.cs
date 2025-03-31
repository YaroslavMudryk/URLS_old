using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Group;

public class GroupInviteEditModel : GroupInviteCreateModel
{
    [Required]
    public Guid Id { get; set; }
}
