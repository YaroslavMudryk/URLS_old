namespace URLS.Application.ViewModels.Audit;

public class AuditViewModel<T>
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Entity { get; set; }
    public string EntityId { get; set; }
    public T Before { get; set; }
    public T After { get; set; }
}
