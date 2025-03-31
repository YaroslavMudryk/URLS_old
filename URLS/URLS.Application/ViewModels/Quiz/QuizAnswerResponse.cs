namespace URLS.Application.ViewModels.Quiz;

public class QuizAnswerResponse
{
    public int QuestionId { get; set; }
    public long[] AnswerIds { get; set; }
    
    public DateTime? ChoiceAt { get; set; }
}
