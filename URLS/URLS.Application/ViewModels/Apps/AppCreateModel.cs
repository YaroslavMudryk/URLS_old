using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Apps;

public class AppCreateModel
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; }
    [StringLength(15)]
    public string ShortName { get; set; }
    [StringLength(1000)]
    public string Description { get; set; }
    [StringLength(1000)]
    public string Image { get; set; }
    [Required]
    public bool IsActive { get; set; }
    [Required]
    public DateTime ActiveFrom { get; set; }
    [Required]
    public DateTime ActiveTo { get; set; }

    public AppCreateModel(AppViewModel app)
    {
        Name = app.Name;
        ShortName = app.ShortName;
        Description = app.Description;
        Image = app.Image;
        IsActive = app.IsActive;
        ActiveFrom = app.ActiveFrom;
        ActiveTo = app.ActiveTo;
    }

    public AppCreateModel()
    {

    }
}
