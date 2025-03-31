using System.ComponentModel.DataAnnotations;

namespace URLS.Domain.Models;

public class Timetable : BaseModel<long>
{
    public int? TeacherId { get; set; }
    public User Teacher { get; set; }
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }
    [Required]
    public int GroupId { get; set; }
    [Required]
    public bool IsHoliday { get; set; }
    public Holiday Holiday { get; set; }
    [Required]
    public LessonType Type { get; set; }
    [Required]
    public LessonTime Time { get; set; }
    [Required]
    public DateTime Date { get; set; }
}
