namespace SummitGrid.Core.Entities;

public class Incident
{
    public int Id {get; set; }
    
    public string Title {get; set; } = string.Empty;
    
    public string Description {get; set; } = string.Empty;
    
    public IncidentType IncidentType {get; set; }
    
    public IncidentStatus IncidentStatus {get; set; }
    
    public int ClimbingAreaId {get; set; }
    public ClimbingArea ClimbingArea {get; set; } = null!;

    public int? RouteId {get; set; }
    public Route? Route {get; set; }

    public string? ReportedBy {get; set; }

    public string? AssignedTo {get; set; }

    public DateTime CreatedAt {get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt {get; set; } = DateTime.UtcNow;
}