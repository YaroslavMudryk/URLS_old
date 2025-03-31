using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Post.Comment;

namespace URLS.Application.Services.Interfaces;

public interface ICommentService
{
    Task<Result<List<CommentViewModel>>> GetCommentsByPostIdAsync(int groupId, int postId, int skip = 0, int count = 20);
    Task<Result<CommentViewModel>> CreateCommentAsync(CommentCreateModel model);
    Task<Result<CommentViewModel>> UpdateCommentAsync(CommentEditModel model);
    Task<Result<bool>> RemoveCommentAsync(int groupId, int postId, long commentId);
}
