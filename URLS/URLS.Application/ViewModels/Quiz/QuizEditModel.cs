using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Quiz;

public class QuizEditModel : QuizCreateModel
{
    [Required]
    public Guid Id { get; set; }
}
