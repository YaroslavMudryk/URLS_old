using System.Text.Json.Serialization;

namespace URLS.Application.ViewModels.News;

public class CommonNewsResponse
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("long_title")]
    public string FullTitle { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; }
    [JsonPropertyName("ref_url")]
    public string RefUrl { get; set; }
    [JsonPropertyName("data")]
    public List<CommonItemNewsResponse> Data { get; set; }

    public void Sort(DateTime? date = null)
    {
        if (date != null)
        {
            var itemByDate = Data.FirstOrDefault(s => s.At.Date == date.Value.Date);
            Data = new List<CommonItemNewsResponse> { itemByDate };
        }
    }

    private void SortDataByDesc()
    {
        Data = Data.OrderByDescending(s => s.At).ToList();
    }

    public void CalculateWarData()
    {
        var sortedItems = Data.OrderByDescending(s => s.At).ToArray();
        for (int i = 0; i < Data.Count; i++)
        {
            if (i + 1 >= Data.Count)
                break;

            var currentItem = sortedItems[i];
            var previewItem = sortedItems[i + 1];
            var res = currentItem.Val - previewItem.Val;
            if (res > 0)
                currentItem.Diff = $"+{res}";
            else
                currentItem.Diff = null;
        }
        Data = sortedItems.ToList();
    }

    public void CalculateCovidData()
    {
        int counter = 0;
        foreach (var item in Data)
        {
            counter += item.Val;
            item.Val = counter;
        }

        var sortedItems = Data.OrderByDescending(s => s.At).ToArray();
        for (int i = 0; i < Data.Count; i++)
        {
            if (i + 1 >= Data.Count)
                break;

            var currentItem = sortedItems[i];
            var previewItem = sortedItems[i + 1];
            var res = currentItem.Val - previewItem.Val;
            if (res > 0)
                currentItem.Diff = $"+{res}";
            else
                currentItem.Diff = null;
        }
        Data = sortedItems.ToList();
    }
}
