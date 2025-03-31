using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
namespace URLS.Infrastructure.Data.Extensions;

public static class JsonConvertorExtensions
{
    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };
    public static string ToJson(this object data)
    {
        return JsonSerializer.Serialize(data, _options);
    }
    public static T FromJson<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json, _options);
    }
}
