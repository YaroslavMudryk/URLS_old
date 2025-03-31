using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.User;

public class UserCreateModel
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string FirstName { get; set; }
    [StringLength(100, MinimumLength = 1)]
    public string MiddleName { get; set; }
    [Required, StringLength(100, MinimumLength = 1)]
    public string LastName { get; set; }
    [StringLength(50, MinimumLength = 4)]
    public string UserName { get; set; }
    [Required, EmailAddress, StringLength(200, MinimumLength = 5)]
    public string Login { get; set; }
    [Required, StringLength(50, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$")]
    public string Password { get; set; }
    [Required]
    public int RoleId { get; set; }
}
