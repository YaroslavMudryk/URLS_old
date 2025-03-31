using URLS.Domain.Models;
using Extensions.DeviceDetector.Models;

namespace URLS.Application.ViewModels.Session;

public class SessionViewModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public AppModel App { get; set; }
    public ClientInfo Client { get; set; }
    public Domain.Models.Location Location { get; set; }
    public bool IsActive { get; set; }
    public bool ViaMFA { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime? DeactivatedAt { get; set; }
}
