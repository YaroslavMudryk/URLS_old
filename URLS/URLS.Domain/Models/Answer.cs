using System.ComponentModel.DataAnnotations;
namespace URLS.Domain.Models;

public class Answer : BaseModel<long>
{
    [Required, StringLength(500, MinimumLength = 1)]
    public string Response { get; set; }
    [Required]
    public bool IsCorrect { get; set; }
    public int QuestionId { get; set; }
    public Question Question { get; set; }
}
