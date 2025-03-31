using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Quiz;

namespace URLS.Application.Services.Interfaces;

public interface IQuizService
{
    Task<Result<List<QuizViewModel>>> GetAllAsync(int offset = 0, int count = 20);
    Task<Result<List<QuizViewModel>>> GetBySubjectIdAsync(int subjectId, int offset = 0, int count = 10);
    Task<Result<QuizViewModel>> GetByIdAsync(Guid id, bool fullTest = false);
    Task<Result<List<QuizResultViewModel>>> GetResultsAsync(Guid quizId, int offset = 0, int count = 10);
    Task<Result<QuizResultViewModel>> GetResultAsync(Guid quizId, int quizResultId);
    Task<Result<QuizStatisticsViewModel>> GetQuizStatisticsAsync(Guid quizId);
    Task<Result<List<QuizResultViewModel>>> GetUserResultsAsync(int userId, int offset = 0, int count = 10);
    Task<Result<QuizViewModel>> CreateAsync(QuizCreateModel quiz);
    Task<Result<QuizViewModel>> UpdateAsync(QuizEditModel quiz);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<QuizStartedViewModel>> StartQuizAsync(Guid quizId);
    Task<Result<QuizResultViewModel>> FinishQuizAsync(int quizResultId, QuizAnswerCreateModel model);
}
