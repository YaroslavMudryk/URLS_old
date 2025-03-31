using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Post;

namespace URLS.Application.Services.Interfaces;

public interface IPostService
{
    Task<Result<List<PostViewModel>>> GetPostsByGroupIdAsync(int groupId, int skip = 0, int count = 20);
    Task<Result<PostViewModel>> GetPostByIdAsync(int postId, int groupId);
    Task<Result<PostViewModel>> CreatePostAsync(PostCreateModel model);
    Task<Result<PostViewModel>> UpdatePostAsync(PostEditModel model);
    Task<Result<bool>> RemovePostAsync(int postId, int groupId);
}
