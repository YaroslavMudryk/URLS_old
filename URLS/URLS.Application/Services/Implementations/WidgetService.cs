using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.News;

namespace URLS.Application.Services.Implementations;

public class WidgetService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache) : IWidgetService
{
    private const int _defaultSecondsCache = 7200;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("UaData");
    private readonly string[] _warTypes = new[] {
        "people",
        "tanks",
        "artilery",
        "auto",
        "bbm",
        "bpla",
        "helicopters",
        "planes",
        "ppo",
        "rszv",
        "ships",
    };
    private readonly string[] _covidTypes = new[]
    {
        "total-cases",
        "totla-deaths",
        "persons-vaccinated",
        "persons-fully-vaccinated",
        "persons-with-booster"
    };

    public async Task<Result<CovidNewsReponse>> GetCovidInfoAsync(DateTime? date = null)
    {
        string cacheKey = "covid";
        try
        {
            if (memoryCache.TryGetValue(cacheKey, out CovidNewsReponse covidNewsReponse))
            {
                return Result<CovidNewsReponse>.SuccessWithData(covidNewsReponse);
            }
            else
            {
                var result = new CovidNewsReponse();
                result.Disease = new List<CommonNewsResponse>();
                foreach (var type in _covidTypes)
                {
                    var httpResponse = await _httpClient.GetAsync($"coronavirus-in-ukraine/{type}.json");
                    var stringJson = await httpResponse.Content.ReadAsStringAsync();
                    var item = JsonSerializer.Deserialize<CommonNewsResponse>(stringJson);

                    item.CalculateCovidData();
                    item.Sort(date);

                    result.Disease.Add(item);
                }
                memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_defaultSecondsCache)
                });
                return Result<CovidNewsReponse>.SuccessWithData(result);
            }
        }
        catch (Exception ex)
        {
            return Result<CovidNewsReponse>.Exception(ex);
        }
    }

    public async Task<Result<WarNewsResponse>> GetWarInfoAsync(DateTime? date = null)
    {
        string cacheKey = "war";
        try
        {
            if (memoryCache.TryGetValue(cacheKey, out WarNewsResponse warNewsResponse))
            {
                return Result<WarNewsResponse>.SuccessWithData(warNewsResponse);
            }
            else
            {
                var result = new WarNewsResponse();
                result.Losses = new List<CommonNewsResponse>();
                foreach (var type in _warTypes)
                {
                    var httpResponse = await _httpClient.GetAsync($"ukraine-russia-war-2022/{type}.json");
                    var stringJson = await httpResponse.Content.ReadAsStringAsync();
                    var item = JsonSerializer.Deserialize<CommonNewsResponse>(stringJson);

                    item.CalculateWarData();
                    item.Sort(date);

                    result.Losses.Add(item);
                }
                memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_defaultSecondsCache)
                });
                return Result<WarNewsResponse>.SuccessWithData(result);
            }
        }
        catch (Exception ex)
        {
            return Result<WarNewsResponse>.Exception(ex);
        }
    }
}
