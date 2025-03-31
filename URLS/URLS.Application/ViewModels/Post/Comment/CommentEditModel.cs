using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Post.Comment;

public class CommentEditModel : CommentCreateModel
{
    [Required]
    public long Id { get; set; }
}
