using URLS.Application.Options;

namespace URLS.Application.Extensions;

public static class SearchOptionsExtensions
{
    private static int DefaultCount = 20;
    private static int MaxCount = DefaultCount * 4;
    private static int DefaultOffset = 0;
    public static void PrepareOptions(this SearchOptions searchOptions)
    {
        var count = searchOptions.Count;
        var offset = searchOptions.Offset;

        if (count <= 0 && count > MaxCount)
            searchOptions.Count = DefaultCount;

        if (offset < 0)
            searchOptions.Offset = DefaultOffset;
    }
}
