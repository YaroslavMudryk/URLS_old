namespace URLS.Constants.APIResponse;

public class Meta
{
    public int TotalCount { get; set; }
    public int Count { get; set; }

    public int TotalPages { get; set; }
    public int Page { get; set; }

    public static Meta FromMeta(int totalCount, int offset, int limit)
    {
        var meta = new Meta();
        meta.TotalCount = totalCount;
        meta.Count = limit;
        meta.TotalPages = totalCount == 0 || limit == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)limit);
        meta.Page = offset == 0 || limit == 0 ? 0 : (int)Math.Ceiling((offset / (double)limit)) + 1;
        return meta;
    }
}
