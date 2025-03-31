using URLS.Application.ViewModels.Post;
using URLS.Application.ViewModels.User;

namespace URLS.Application.ViewModels.Reaction;

public class ReactionViewModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public PostViewModel Post { get; set; }
    public UserViewModel From { get; set; }
    public string Reaction { get; set; }
}
