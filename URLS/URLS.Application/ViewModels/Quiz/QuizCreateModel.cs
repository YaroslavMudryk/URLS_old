using System.ComponentModel.DataAnnotations;
using URLS.Domain.Models;

namespace URLS.Application.ViewModels.Quiz;

public class QuizCreateModel
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
    public List<QuestionEditModel> Questions { get; set; }
}
