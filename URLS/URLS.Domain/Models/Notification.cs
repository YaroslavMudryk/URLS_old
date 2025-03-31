namespace URLS.Domain.Models;

public class Notification : BaseModel<long>
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string ImageUrl { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public NotificationType Type { get; set; }
    public bool IsImportant { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
}

public enum NotificationType
{
    Welcome,
    AcceptedInGroup,
    NewLogin,
    LoginAttempt,
    ChangePassword,
    Logout,
    NewPost,
    Locked,
    PermissionChanged,
    RolesChanged
}
