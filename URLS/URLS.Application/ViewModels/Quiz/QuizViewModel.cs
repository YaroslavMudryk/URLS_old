using URLS.Application.ViewModels.Subject;
using URLS.Domain.Models;

namespace URLS.Application.ViewModels.Quiz;

public class QuizViewModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public QuizConfig Config { get; set; }
    public AuthorModel Author { get; set; }
    public SubjectViewModel Subject { get; set; }
    public List<QuestionViewModel> Questions { get; set; }
}
