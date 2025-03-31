namespace URLS.Application.Options;

public class SearchSubjectOptions : SearchOptions
{
    public int? GroupId { get; set; } = null;
    public string Name { get; set; } = null;
    public bool? IsCurrentSemestr { get; set; } = null;
    public bool? IsTemplate { get; set; } = null;
    public IEnumerable<int> SubjectIds { get; set; } = null;
}
