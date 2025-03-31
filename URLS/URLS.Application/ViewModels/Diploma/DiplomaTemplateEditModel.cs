using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Diploma;

public class DiplomaTemplateEditModel : DiplomaTemplateCreateModel
{
    [Required]
    public string Id { get; set; }
}
