namespace URLS.Application.ViewModels.Apps;

public class AppViewModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string CreatedFromIP { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public bool IsActive { get; set; }
    public DateTime ActiveFrom { get; set; }
    public DateTime ActiveTo { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public string LastUpdatedBy { get; set; }
    public string LastUpdatedFromIP { get; set; }
}
