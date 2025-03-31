using System.ComponentModel.DataAnnotations;
namespace URLS.Domain.Models;

public class Question : BaseModel<int>
{
    [Required, StringLength(500, MinimumLength = 1)]
    public string QuestionText { get; set; }
    [Required]
    public int Index { get; set; }
    public bool IsMultipleAnswers { get; set; }
    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; }
    public List<Answer> Answers { get; set; }
}
