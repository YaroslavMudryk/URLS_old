using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Quiz;

public class QuestionCreateModel
{
    [Required, StringLength(500, MinimumLength = 1)]
    public string QuestionText { get; set; }
    [Required]
    public int Index { get; set; }
    [Required]
    public bool IsMultipleAnswers { get; set; }
    public List<AnswerEditModel> Answers { get; set; }
}
