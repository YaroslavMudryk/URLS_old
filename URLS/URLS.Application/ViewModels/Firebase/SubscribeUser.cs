namespace URLS.Application.ViewModels.Firebase;

public class SubscribeUser
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public List<FirebaseDevice> Devices { get; set; }
}
