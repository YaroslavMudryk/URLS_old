using URLS.Application.ViewModels.Reaction;
using URLS.Domain.Models;

namespace URLS.Application.Helpers;

public class ReactionHelper
{
    public static string GetReactionFromId(int reactionId)
    {
        if (reactionId == 1)
            return ReactionData.Like;
        if (reactionId == 2)
            return ReactionData.Dislike;
        if (reactionId == 3)
            return ReactionData.Heart;
        if (reactionId == 4)
            return ReactionData.Congratulations;
        if (reactionId == 5)
            return ReactionData.Laughter;
        if (reactionId == 6)
            return ReactionData.Shit;
        if (reactionId == 7)
            return ReactionData.Swearing;
        if (reactionId == 8)
            return ReactionData.Cry;
        if (reactionId == 9)
            return ReactionData.Wow;
        return string.Empty;
    }

    public static bool ValidateReactionId(int reactionId)
    {
        if (reactionId >= 1 && reactionId <= ReactionData.MaxReactionId)
            return true;
        return false;
    }

    public static bool ValidateReactionByPost(int reactionId, Post post)
    {
        if (post.IsAvailableReactions && post.AvailableReactionIds == null)
            return ValidateReactionId(reactionId);

        if (post.IsAvailableReactions && post.AvailableReactionIds != null)
            return post.AvailableReactionIds.Contains(reactionId);

        if (!post.IsAvailableReactions)
            return false;

        return true;
    }

    public static bool ValidateReactionIds(int[] ids)
    {
        if (ids == null || ids.Length == 0)
            return true;
        return ids.All(s => s >= 1 && s <= ReactionData.MaxReactionId);
    }

    public static Dictionary<int, string> MapAvailableReactions(int[] reactionIds)
    {
        var allReactions = ReactionData.GetAllReactions();
        if (reactionIds == null || reactionIds.Length == 0)
            return allReactions;

        var availableReactions = allReactions.Where(s => reactionIds.Contains(s.Key));

        return availableReactions.ToDictionary(s => s.Key, s => s.Value);
    }
}
