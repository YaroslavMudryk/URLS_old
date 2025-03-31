using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Identity;

public class AppLoginCreateModel
{
    [Required, StringLength(30)]
    public string Id { get; set; }
    [Required, StringLength(70)]
    public string Secret { get; set; }
    [Required, StringLength(50, MinimumLength = 1)]
    public string Version { get; set; }
}
