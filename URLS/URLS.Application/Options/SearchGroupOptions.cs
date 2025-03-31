namespace URLS.Application.Options;

public class SearchGroupOptions : SearchOptions
{
    public string Name { get; set; } = null;
    public int? Course { get; set; }
    public int? SpecialtyId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
