using Microsoft.AspNetCore.Http;
using System.Text.Json;
using URLS.Constants.Extensions;

namespace URLS.Constants.Localisation;

public class LocalizeService : ILocalizeService
{
    private readonly string[] _languages = new string[] { "en", "uk" };
    private readonly Dictionary<string, List<Dictionary<string, string>>> _localization;
    private readonly HttpContext _httpContext;

    public string[] SupportLanguages => _languages;

    public LocalizeService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext;
        _localization = new Dictionary<string, List<Dictionary<string, string>>>();
        LoadLanguages();
        LoadValues();
    }

    private void LoadLanguages()
    {
        foreach (var language in _languages)
        {
            _localization.Add(language, new List<Dictionary<string, string>>());
        }
    }

    private void LoadValues()
    {
        foreach (var language in _languages)
        {
            var path = Path.Combine(@".\wwwroot\", $@"Resources\SharedResource.{language}.json");

            var json = new StreamReader(path).ReadToEnd();
            var translate = JsonSerializer.Deserialize<ResourceList>(json);

            _localization[language].Add(translate.Resources.ToDictionary(s => s.Key, s => s.Value));
        }
    }

    public string Get(string key)
    {
        key = key.ToLower();
        string response = null;
        var lang = _httpContext.GetLanguage();

        if (_localization.TryGetValue(lang, out var value))
        {
            value.ForEach(s =>
            {
                s.TryGetValue(key, out response);
            });
        }

        CheckResponseLang(key, response);

        return response;
    }

    private void CheckResponseLang(string key, string response)
    {
        if (string.IsNullOrEmpty(response))
            if (_localization.TryGetValue("en", out var value))
            {
                value.ForEach(s =>
                {
                    s.TryGetValue(key, out response);
                });
            }
    }
}
