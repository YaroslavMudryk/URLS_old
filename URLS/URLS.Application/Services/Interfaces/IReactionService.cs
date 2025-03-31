using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Reaction;

namespace URLS.Application.Services.Interfaces;

public interface IReactionService
{
    Task<Result<ReactionViewModel>> CreateAsync(ReactionCreateModel reaction);
    Task<Result<bool>> DeleteAsync(ReactionCreateModel reaction);
    Task<Result<List<ReactionViewModel>>> GetAllByPostIdAsync(int postId, int offset = 0, int count = 20);
    Task<Result<ReactionStatistics>> GetAllReactionsByPostIdAsync(int postId);
    Result<Dictionary<int, string>> GetAllReactions();
}
