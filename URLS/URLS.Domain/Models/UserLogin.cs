using System.ComponentModel.DataAnnotations;

namespace URLS.Domain.Models;

public class UserLogin : BaseModel<int>
{
    [Required, StringLength(100)]
    public string ExternalProvider { get; set; } //Google
    [StringLength(150)]
    public string Email { get; set; } //yaro@gmail.com
    [StringLength(150)]
    public string Key { get; set; } //1524523546545

    public int UserId { get; set; }
    public User User { get; set; }
}
