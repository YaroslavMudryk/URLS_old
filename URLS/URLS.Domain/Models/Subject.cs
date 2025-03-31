using System.ComponentModel.DataAnnotations;
namespace URLS.Domain.Models;

public class Subject : BaseModel<int>
{
    [Required, StringLength(200, MinimumLength = 1)]
    public string Name { get; set; }
    [StringLength(1000)]
    public string Description { get; set; }
    [Required]
    public int Semestr { get; set; }
    [Required]
    public DateTime From { get; set; }
    [Required]
    public DateTime To { get; set; }
    [Required]
    public bool IsTemplate { get; set; }
    public SubjectConfig Config { get; set; }

    public int? GroupId { get; set; }
    public Group Group { get; set; }

    public int TeacherId { get; set; }
    public User Teacher { get; set; }
    public List<Lesson> Lessons { get; set; }
    public List<Timetable> Timetables { get; set; }
    public List<Report> Reports { get; set; }
}
