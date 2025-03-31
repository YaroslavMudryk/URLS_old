using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.University;

public class UniversityCreateModel
{
    [Required, StringLength(250)]
    public string Name { get; set; }
    [StringLength(15)]
    public string ShortName { get; set; }
    [StringLength(250)]
    public string NameEng { get; set; }
    [StringLength(15)]
    public string ShortNameEng { get; set; }

    public UniversityCreateModel()
    {

    }

    public UniversityCreateModel(UniversityViewModel model)
    {
        Name = model.Name;
        ShortName = model.ShortName;
        NameEng = model.NameEng;
        ShortNameEng = model.ShortNameEng;
    }
}
