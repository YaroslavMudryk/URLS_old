using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Firebase;

public class SubscribeModel
{
    [Required]
    public string Token { get; set; }
    [Required, Range(1,3)]
    public int Type { get; set; }
}
