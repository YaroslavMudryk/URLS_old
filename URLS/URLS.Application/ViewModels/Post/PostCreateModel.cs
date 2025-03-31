using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Post;

public class PostCreateModel
{
    [Required, StringLength(150, MinimumLength = 1)]
    public string Title { get; set; }
    [Required, StringLength(10000, MinimumLength = 5)]
    public string Content { get; set; }
    [Required]
    public bool IsImportant { get; set; }
    [Required]
    public bool AvailableToComment { get; set; }
    [Required]
    public bool IsPublic { get; set; }
    public bool IsAvailableReactions { get; set; }
    public int[] AvailableReactionIds { get; set; }
    [Required]
    public int GroupId { get; set; }
}
