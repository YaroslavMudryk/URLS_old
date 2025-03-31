using URLS.Domain.Models;

namespace URLS.Application.ViewModels.Group.GroupRole;

public class UserGroupRoleViewModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; }
    public string NameEng { get; set; }
    public string Color { get; set; }
    public string Description { get; set; }
    public string DescriptionEng { get; set; }
    public UserGroupPermission Permissions { get; set; }
}
