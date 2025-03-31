using System.ComponentModel.DataAnnotations;

namespace URLS.Domain.Models;

public class UserGroupRole : BaseModel<int>
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; }
    [StringLength(100, MinimumLength = 2)]
    public string NameEng { get; set; }
    [Required, StringLength(10, MinimumLength = 2)]
    public string Color { get; set; }
    [StringLength(250, MinimumLength = 2)]
    public string Description { get; set; }
    [StringLength(250, MinimumLength = 2)]
    public string DescriptionEng { get; set; }
    [Required]
    public bool CanEdit { get; set; }
    [Required]
    public string UniqId { get; set; }
    public UserGroupPermission Permissions { get; set; }
    public List<UserGroup> UserGroups { get; set; }
}

public class UserGroupPermission
{
    public bool CanCreatePost { get; set; }
    public bool CanUpdatePost { get; set; }
    public bool CanUpdateAllPosts { get; set; }
    public bool CanRemovePost { get; set; }
    public bool CanRemoveAllPosts { get; set; }

    public bool CanCreateComment { get; set; }
    public bool CanOpenCloseComment { get; set; }
    public bool CanRemoveComment { get; set; }
    public bool CanRemoveAllComments { get; set; }

    public bool CanCreateInviteCode { get; set; }
    public bool CanUpdateInviteCode { get; set; }
    public bool CanRemoveInviteCode { get; set; }
    public bool CanViewInviteCodes { get; set; }


    public bool CanUpdateImage { get; set; }
    public bool CanEditInfo { get; set; }
}
