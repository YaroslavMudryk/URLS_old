namespace URLS.Application.ViewModels.Firebase;

public class FirebaseDevice
{
    public string Token { get; set; }
    public string Type { get; set; }
    public DeviceType DeviceType { get; set; }

    public static FirebaseDevice AsAndroid(string token)
    {
        return new FirebaseDevice
        {
            Token = token,
            Type = "Andoid",
            DeviceType = DeviceType.Android
        };
    }

    public static FirebaseDevice AsIOS(string token)
    {
        return new FirebaseDevice
        {
            Token = token,
            Type = "IOS",
            DeviceType= DeviceType.IOS
        };
    }

    public static FirebaseDevice AsWeb(string token)
    {
        return new FirebaseDevice
        {
            Token = token,
            Type = "Web",
            DeviceType = DeviceType.Web
        };
    }
}

public enum DeviceType
{
    Android = 1,
    IOS = 2,
    Web = 3
}
