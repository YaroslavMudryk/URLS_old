namespace URLS.Application.ViewModels.Audit;

public class AuditCreateModel
{
    public string EntityId { get; set; }
    public string Entity { get; set; }
    public object Before { get; set; }
    public object After { get; set; }
}
