namespace URLS.Constants.Localisation;

public class ResourceList
{
    public string Language { get; set; }
    public List<Resource> Resources { get; set; }
}

public class Resource
{
    public string Key { get; set; }
    public string Value { get; set; }
}
