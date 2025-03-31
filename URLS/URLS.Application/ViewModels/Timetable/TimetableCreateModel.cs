using URLS.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Timetable;

public class TimetableCreateModel
{
    public long? Id { get; set; }
    [Required]
    public int TeacherId { get; set; }
    [Required]
    public int SubjectId { get; set; }
    public int? HolidayId { get; set; }
    [Required]
    public int TimeId { get; set; }
    [Required]
    public LessonType Type { get; set; }
    [Required]
    public DateTime Date { get; set; }
    public int GroupId { get; set; }
}
