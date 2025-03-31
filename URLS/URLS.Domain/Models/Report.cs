using System.ComponentModel.DataAnnotations;

namespace URLS.Domain.Models;

public class Report : BaseModel<int>
{
    [StringLength(100, MinimumLength = 2)]
    public string Title { get; set; }
    [Required]
    public bool IsDraft { get; set; }
    [Required]
    public ReportType Type { get; set; }
    public List<Student> CalculatedMarks { get; set; }
    public List<Student> Marks { get; set; }
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }
}

public enum ReportType
{
    Intermediate = 1,
    Finall = 2
}
