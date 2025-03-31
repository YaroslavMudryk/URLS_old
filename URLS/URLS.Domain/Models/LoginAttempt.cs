using Extensions.DeviceDetector.Models;
using System.ComponentModel.DataAnnotations;
namespace URLS.Domain.Models;

public class LoginAttempt: BaseModel<int>
{
    [Required, StringLength(150)]
    public string Login { get; set; }
    [Required]
    public bool IsSuccess { get; set; }
    public ClientInfo Client { get; set; }
    public string Password { get; set; }
}
