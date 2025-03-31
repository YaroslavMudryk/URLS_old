namespace URLS.Application.Extensions;

public static class ClaimExtensions
{
    public static string GetHashForClaimIds(this IEnumerable<int> claimIds)
    {
        return string.Join("", claimIds.OrderBy(s => s));
    }
}
