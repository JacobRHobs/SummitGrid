using SummitGrid.Core.Entities;

namespace SummitGrid.Api.Models;

public class IncidentResponse
{
    public int Id {get; set; }
    
    public string Title {get; set; } = string.Empty;
    
    public string Description {get; set; } = string.Empty;
    
    public IncidentType IncidentType {get; set; }
    
    public IncidentStatus IncidentStatus {get; set; }
    
    public int ClimbingAreaId {get; set; }

    public int? RouteId {get; set; }

    public string? ReportedBy {get; set; } = string.Empty;

    public string? AssignedTo {get; set; }

    public DateTime CreatedAt {get; set; }

    public DateTime UpdatedAt {get; set; }
}