using System.Text.Json.Serialization;

namespace URLS.Application.ViewModels.News;

public class CommonItemNewsResponse
{
    [JsonPropertyName("at")]
    public DateTime At { get; set; }
    [JsonPropertyName("val")]
    public int Val { get; set; }
    public string Diff { get; set; }
}
