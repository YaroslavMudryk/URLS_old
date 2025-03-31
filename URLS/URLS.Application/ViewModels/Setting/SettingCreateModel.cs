using System.ComponentModel.DataAnnotations;
using URLS.Domain.Models;

namespace URLS.Application.ViewModels.Setting;

public class SettingCreateModel
{
    [Required]
    public DateTime FirtsSemesterStart { get; set; }
    [Required]
    public DateTime FirtsSemesterEnd { get; set; }
    [Required]
    public DateTime SecondSemesterStart { get; set; }
    [Required]
    public DateTime SecondSemesterEnd { get; set; }
    [Required]
    public int MaxCourseInUniversity { get; set; }
    public string DirectorSignature { get; set; }
    public string UniversityStamp { get; set; }
    [Required]
    public IEnumerable<Holiday> Holidays { get; set; }
    [Required]
    public IEnumerable<LessonTime> LessonTimes { get; set; }
}
