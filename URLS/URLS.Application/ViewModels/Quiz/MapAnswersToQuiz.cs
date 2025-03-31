using URLS.Domain.Models;

namespace URLS.Application.ViewModels.Quiz;

public class MapAnswersToQuiz
{
    public List<Question> Questions { get; set; }
    public List<QuizAnswerResponse> QuizResponse { get; set; }
    public QuizResult Result { get; set; }
    public Domain.Models.Quiz Quiz { get; set; }

    public MapAnswersToQuiz()
    {

    }

    public MapAnswersToQuiz(List<Question> questions, List<QuizAnswerResponse> quizResponse, QuizResult result, Domain.Models.Quiz quiz)
    {
        Questions = questions;
        QuizResponse = quizResponse;
        Result = result;
        Quiz = quiz;
    }
}
