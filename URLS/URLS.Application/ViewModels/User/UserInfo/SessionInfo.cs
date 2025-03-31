using URLS.Application.ViewModels.Session;

namespace URLS.Application.ViewModels.User.UserInfo;

public class SessionInfo
{
    public List<SessionViewModel> Sessions { get; set; }
    public int TotalSessions { get; set; }
    public int ActiveSessions { get; set; }
}
