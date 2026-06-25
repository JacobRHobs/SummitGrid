namespace SummitGrid.Core.Entities;

public class AuditLog
{
    public int Id {get; set; }

    public string UserName {get; set; } = string.Empty;

    public AuditAction AuditAction {get; set; }

    public int IncidentId {get; set; }
    public Incident Incident {get; set; } = null!;

    public int OldValue {get; set; }

    public int NewValue {get; set; }

    public DateTime TimeStamp {get; set; } = DateTime.UtcNow;
}