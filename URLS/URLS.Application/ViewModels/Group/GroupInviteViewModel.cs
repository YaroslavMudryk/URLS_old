namespace URLS.Application.ViewModels.Group;

public class GroupInviteViewModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; }
    public DateTime ActiveFrom { get; set; }
    public DateTime ActiveTo { get; set; }
    public string CodeJoin { get; set; }
    public bool IsActive { get; set; }
    public GroupViewModel Group { get; set; }
}
