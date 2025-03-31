namespace URLS.Application.ViewModels.Firebase;

public class PendingRequest
{
    public int UserId { get; set; }
    public List<PushMessage> PushMessages { get; set; }
}
