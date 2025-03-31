using System.ComponentModel.DataAnnotations;

namespace URLS.Domain.Models;

public class Setting : BaseModel<int>
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

public class Holiday
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
}

public class LessonTime
{
    public int Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Number { get; set; }
    public string Description { get; set; }
}
