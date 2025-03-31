using System.ComponentModel.DataAnnotations;
namespace URLS.Domain.Models;

public class Quiz : BaseModel<Guid>
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string Name { get; set; }
    [StringLength(300)]
    public string Description { get; set; }
    public QuizConfig Config { get; set; }
    public AuthorModel Author { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public bool IsAvalible { get; set; }
    public bool IsTemplate { get; set; }
    public int? SubjectId { get; set; }
    public Subject Subject { get; set; }
    public List<Question> Questions { get; set; }
    public List<QuizResult> QuizResults { get; set; }
}
public class QuizConfig
{
    public int MaxAttempts { get; set; } // (0) without limits
    public double MarkPerQuiz { get; set; }
    public int Minutes { get; set; } // (0) without limits
    public bool RandomQuestionsAndAnswers { get; set; }
    public bool ShowResults { get; set; }
    public bool ShowCorrectAnswers { get; set; }
}

public class AuthorModel
{
    public int? UserId { get; set; }
    public string FullName { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
}
