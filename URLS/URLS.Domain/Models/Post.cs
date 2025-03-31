using System.ComponentModel.DataAnnotations;

namespace URLS.Domain.Models;

public class Post : BaseModel<int>
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
    public int[] AvailableReactionIds { get; set; }
    public bool IsAvailableReactions { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int GroupId { get; set; }
    public Group Group { get; set; }
    public List<Comment> Comments { get; set; }
    public List<Reaction> Reactions { get; set; }
}
