using URLS.Domain.Models;

namespace URLS.Application.Services.Interfaces;

public interface IPermissionPostService
{
    Task<bool> CanCreatePostAsync(int groupId);
    Task<bool> CanViewOnlyPublicPostsAsync(int groupId);
    bool CanViewPost(Post post);
    Task<bool> CanViewPostAsync(int postId);
    Task<bool> CanRemovePostAsync(int groupId, Post post);
    Task<bool> CanRemovePostAsync(int groupId, int postId);
    Task<bool> CanUpdatePostAsync(int groupId, Post post);
    Task<bool> CanUpdatePostAsync(int groupId, int postId);
}
