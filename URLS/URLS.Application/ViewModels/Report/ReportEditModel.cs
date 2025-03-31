using URLS.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Report;

public class ReportEditModel
{
    public int Id { get; set; }
    [StringLength(100, MinimumLength = 2)]
    public string Title { get; set; }
    [Required]
    public bool IsDraft { get; set; }
    [Required]
    public ReportType Type { get; set; }
    public List<Student> Marks { get; set; }
    public int SubjectId { get; set; }
}
