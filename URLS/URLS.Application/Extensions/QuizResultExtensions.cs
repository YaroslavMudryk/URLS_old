using URLS.Domain.Models;

namespace URLS.Application.Extensions;

public static class QuizResultExtensions
{
    public static int GetAverageTimeInSeconds(this IEnumerable<QuizResult> quizResults)
    {
        var sum = 0;
        var commonCount = 0;

        foreach (var quizResult in quizResults)
        {
            if (quizResult.EndAt != null)
            {
                var diff = quizResult.EndAt.Value - quizResult.StartAt;
                sum += (int)diff.TotalSeconds;
                commonCount++;
            }
        }

        return sum / commonCount;
    }
}
