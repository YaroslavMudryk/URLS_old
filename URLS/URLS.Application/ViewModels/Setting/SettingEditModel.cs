using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Setting;

public class SettingEditModel : SettingCreateModel
{
    [Required]
    public int Id { get; set; }
}
