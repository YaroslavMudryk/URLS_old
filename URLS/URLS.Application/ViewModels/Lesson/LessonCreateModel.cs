using URLS.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Lesson;

public class LessonCreateModel
{
    [Required, StringLength(250, MinimumLength = 2)]
    public string Theme { get; set; }
    [StringLength(1500, MinimumLength = 1)]
    public string Description { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public LessonType LessonType { get; set; }
    [StringLength(1000, MinimumLength = 1)]
    public string Homework { get; set; }
    [Required]
    public int SubjectId { get; set; }
    public long? PreviewLessonId { get; set; }
    public long? NextLessonId { get; set; }
    public int? SubstituteTeacherId { get; set; }
}
