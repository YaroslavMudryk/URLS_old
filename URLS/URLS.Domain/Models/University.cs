using System.ComponentModel.DataAnnotations;

namespace URLS.Domain.Models;

public class University : BaseModel<int>
{
    [Required, StringLength(250)]
    public string Name { get; set; }
    [StringLength(15)]
    public string ShortName { get; set; }
    [StringLength(250)]
    public string NameEng { get; set; }
    [StringLength(15)]
    public string ShortNameEng { get; set; }
    public List<Faculty> Faculties { get; set; }
}
