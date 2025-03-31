using URLS.Application.ViewModels.User;
using URLS.Domain.Models;

namespace URLS.Application.ViewModels.Quiz;

public class QuizResultViewModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public double Mark { get; set; }
    public int Attempt { get; set; }
    public DateTime StartAt { get; set; }
    public QuizResultStatistics Statistics { get; set; }
    public DateTime? EndAt { get; set; }
    public bool TimeIsExpired { get; set; }
    public UserViewModel User { get; set; }
    public QuizViewModel Quiz { get; set; }
    public List<QuestionModel> Result { get; set; }
}
