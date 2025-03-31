using URLS.Domain.Models;

namespace URLS.Application.ViewModels.Notification;

public class NotificationViewModel
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string ImageUrl { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public NotificationType Type { get; set; }
    public bool IsImportant { get; set; }
}
