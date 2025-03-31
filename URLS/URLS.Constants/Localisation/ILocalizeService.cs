namespace URLS.Constants.Localisation;

public interface ILocalizeService
{
    string[] SupportLanguages { get; }
    string Get(string key);
}
