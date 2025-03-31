using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Quiz;

public class AnswerCreateModel
{
    [Required, StringLength(500, MinimumLength = 1)]
    public string Response { get; set; }
    [Required]
    public bool IsCorrect { get; set; }
}
