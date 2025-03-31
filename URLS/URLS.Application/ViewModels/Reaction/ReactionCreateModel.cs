using System.ComponentModel.DataAnnotations;
namespace URLS.Application.ViewModels.Reaction;

public class ReactionCreateModel
{
    public int PostId { get; set; }
    [Required]
    public int ReactionId { get; set; }
}
