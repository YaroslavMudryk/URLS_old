using System.ComponentModel.DataAnnotations;

namespace URLS.Domain.Models;

public class UserGroup : BaseModel<int>
{
    [Required]
    public bool IsAdmin { get; set; }
    [StringLength(50)]
    public string Title { get; set; }
    [Required]
    public UserGroupStatus Status { get; set; }
    public int UserGroupRoleId { get; set; }
    public UserGroupRole UserGroupRole { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int GroupId { get; set; }
    public Group Group { get; set; }
}

public enum UserGroupStatus
{
    New = 1,
    Member = 2,
    Gona = 3
}
