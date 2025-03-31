using System.ComponentModel.DataAnnotations;

namespace URLS.Domain.Models;

public class App : BaseModel<int>
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; }
    [StringLength(15)]
    public string ShortName { get; set; }
    [StringLength(1000)]
    public string Description { get; set; }
    [StringLength(1000)]
    public string Image { get; set; }
    [StringLength(30)]
    public string AppId { get; set; }
    [StringLength(70)]
    public string AppSecret { get; set; }
    [Required]
    public bool IsActive { get; set; }
    [Required]
    public DateTime ActiveFrom { get; set; }
    [Required]
    public DateTime ActiveTo { get; set; }
    [StringLength(150)]
    public string Scheme { get; set; }
    public int? UserId { get; set; }
    public User User { get; set; }

    public bool IsActiveByTime()
    {
        var now = DateTime.Now;
        return now >= ActiveFrom && now <= ActiveTo;
    }
}
