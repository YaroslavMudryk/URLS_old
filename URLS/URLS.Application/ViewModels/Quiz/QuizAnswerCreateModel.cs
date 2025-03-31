namespace URLS.Application.ViewModels.Quiz;

public class QuizAnswerCreateModel
{
    public Guid QuizId { get; set; }
    public List<QuizAnswerResponse> Responses { get; set; }
}
