using URLS.Application.ViewModels.Group;
using URLS.Application.ViewModels.Post.Comment;
using URLS.Application.ViewModels.Reaction;
using URLS.Application.ViewModels.User;

namespace URLS.Application.ViewModels.Post;

public class PostViewModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsImportant { get; set; }
    public bool AvailableToComment { get; set; }
    public bool IsPublic { get; set; }
    public int CountComments { get; set; }
    public bool IsAvailableReactions { get; set; }
    public Dictionary<int, string> AvailableReactions { get; set; }
    public ReactionStatistics Statistics { get; set; }
    public UserViewModel User { get; set; }
    public GroupViewModel Group { get; set; }
    public List<CommentViewModel> Comments { get; set; }
}
