using SummitGrid.Core.Entities;

namespace SummitGrid.Api.Models;
public class CreateIncidentRequest
{   
    public string Title {get; set; } = string.Empty;
    
    public string Description {get; set; } = string.Empty;
    
    public IncidentType IncidentType {get; set; }
    
    public int ClimbingAreaId {get; set; }

    public int? RouteId {get; set; }

    public string? AssignedTo {get; set; }
    public IncidentStatus IncidentStatus { get; internal set; }
}