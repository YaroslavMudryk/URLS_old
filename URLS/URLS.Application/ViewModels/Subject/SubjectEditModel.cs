using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Subject;

public class SubjectEditModel : SubjectCreateModel
{
    [Required]
    public int Id { get; set; }
}
