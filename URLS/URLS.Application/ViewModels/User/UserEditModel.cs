using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.User;

public class UserEditModel
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string FirstName { get; set; }
    [StringLength(100, MinimumLength = 1)]
    public string MiddleName { get; set; }
    [Required, StringLength(100, MinimumLength = 1)]
    public string LastName { get; set; }
}
