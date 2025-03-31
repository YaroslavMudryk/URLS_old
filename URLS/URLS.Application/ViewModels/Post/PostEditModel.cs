using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Post;

public class PostEditModel : PostCreateModel
{
    [Required]
    public int Id { get; set; }
}
