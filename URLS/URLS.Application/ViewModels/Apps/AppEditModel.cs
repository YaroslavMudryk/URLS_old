using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Apps;

public class AppEditModel : AppCreateModel
{
    [Required]
    public int Id { get; set; }

    public AppEditModel(AppViewModel app) : base(app)
    {
        Id = app.Id;
    }

    public AppEditModel()
    {

    }
}
