namespace URLS.Domain.Models;

public class Reaction : BaseModel<Guid>
{
    public int ReactionTypeId { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
    public int? FromId { get; set; }
    public User From { get; set; }
}
