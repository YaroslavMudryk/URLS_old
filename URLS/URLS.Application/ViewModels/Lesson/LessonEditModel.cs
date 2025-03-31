using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Lesson;

public class LessonEditModel : LessonCreateModel
{
    [Required]
    public long Id { get; set; }
}
