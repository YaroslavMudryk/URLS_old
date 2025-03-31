using Extensions.DeviceDetector.Models;
using System.ComponentModel.DataAnnotations;

namespace URLS.Domain.Models;

public class Session : BaseModelWithoutIdentity<Guid>
{
    public AppModel App { get; set; }
    public ClientInfo Client { get; set; }
    public Location Location { get; set; }
    [Required]
    public bool IsActive { get; set; }
    public string Type { get; set; }
    public bool ViaMFA { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    public Guid? DeactivatedBySessionId { get; set; }
    [StringLength(5000, MinimumLength = 5)]
    public string Token { get; set; }
    public DateTime ExpiredAt { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}
