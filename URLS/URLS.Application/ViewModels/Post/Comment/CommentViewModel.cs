using URLS.Application.ViewModels.User;

namespace URLS.Application.ViewModels.Post.Comment;

public class CommentViewModel
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Text { get; set; }
    public bool IsPublic { set; get; }
    public UserViewModel User { get; set; }
    public PostViewModel Post { get; set; }
}
