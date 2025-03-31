namespace URLS.Application.Services.Interfaces;

public interface IPermissionCommentService
{
    Task<bool> CanCreateCommentAsync(int groupId);
    Task<bool> CanViewAllCommentsAsync(int groupId, int postId);
}
