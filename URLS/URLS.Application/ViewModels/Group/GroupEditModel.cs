using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Group;

public class GroupEditModel : GroupCreateModel
{
    [Required]
    public int Id { get; set; }
}
