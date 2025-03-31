using URLS.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Subject;

public class SubjectCreateModel
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
    public int TeacherId { get; set; }
    public int? GroupId { get; set; }
}
