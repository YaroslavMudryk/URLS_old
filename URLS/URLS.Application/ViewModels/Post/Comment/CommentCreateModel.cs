using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Post.Comment;

public class CommentCreateModel
{
    [Required, StringLength(250, MinimumLength = 1)]
    public string Text { get; set; }
    [Required]
    public bool IsPublic { set; get; }
    public int PostId { get; set; }
    public int GroupId { get; set; }
}
