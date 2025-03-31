using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace URLS.Domain.Models;

public class User : BaseModel<int>
{
    [Required, StringLength(100, MinimumLength = 1), PersonalData]
    public string FirstName { get; set; }
    [StringLength(100, MinimumLength = 1), PersonalData]
    public string MiddleName { get; set; }
    [Required, StringLength(100, MinimumLength = 1), PersonalData]
    public string LastName { get; set; }
    [StringLength(50, MinimumLength = 3)]
    public string UserName { get; set; }
    [Required, EmailAddress, StringLength(200, MinimumLength = 5)]
    public string Login { get; set; }
    [Required, StringLength(1500, MinimumLength = 1)]
    public string PasswordHash { get; set; }
    [Required]
    public bool FromImport { get; set; }
    public DateTime? ModifiedFromTemp { get; set; }
    public bool MFA { get; set; }
    public string MFASecretKey { get; set; }
    [StringLength(1000, MinimumLength = 1)]
    public string Image { get; set; }
    [EmailAddress, StringLength(150, MinimumLength = 3)]
    public string ContactEmail { get; set; }
    [Phone, StringLength(15, MinimumLength = 9)]
    public string ContactPhone { get; set; }
    [Required]
    public DateTime JoinAt { get; set; }
    [Required]
    public int AccessFailedCount { get; set; }
    [Required]
    public bool LockoutEnabled { get; set; }
    public DateTime? LockoutEnd { get; set; }
    [Required]
    public bool IsActivateAccount { get; set; }
    public NotificationSettings NotificationSettings { get; set; }
    public List<Session> Sessions { get; set; }
    public List<Notification> Notifications { get; set; }
    public List<UserGroup> UserGroups { get; set; }
    public List<Comment> Comments { get; set; }
    public List<Subject> Subjects { get; set; }
    public List<UserLogin> UserLogins { get; set; }
    public List<Diploma> Diplomas { get; set; }
    public List<Lesson> Lessons { get; set; }
    public List<Timetable> Timetables { get; set; }
    public List<App> Apps { get; set; }
    public List<QuizResult> QuizResults { get; set; }
    public List<Reaction> Reactions { get; set; }
    public List<MFA> MFAs { get; set; }

    public User(string firstName, string middleName, string lastName, string login, string userName)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Login = login;
        UserName = userName;
        JoinAt = DateTime.Now;
        AccessFailedCount = 0;
        FromImport = false;
    }
    public User() { }

    public bool IsLocked()
    {
        var isLocked = false;
        if (LockoutEnd == null)
            isLocked = false;
        if (LockoutEnd.HasValue)
        {
            if (LockoutEnd.Value > DateTime.Now)
                isLocked = true;
            else
            {
                LockoutEnd = null;
                isLocked = false;
            }
        }
        return isLocked;
    }

    public void SetLock(bool isAvtive)
    {
        AccessFailedCount = 0;
        LockoutEnd = null;
        LockoutEnabled = isAvtive;
    }
}

public class NotificationSettings
{
    public bool Welcome { get; set; }
    public bool AcceptedInGroup { get; set; }
    public bool NewLogin { get; set; }
    public bool ChangePassword { get; set; }
    public bool Logout { get; set; }
    public bool NewPost { get; set; }
}
