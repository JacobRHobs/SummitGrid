using SummitGrid.Core.Entities;

namespace SummitGrid.Core.Models;

public class CreateIncidentCommand
{
    public string UserName {get; set; } = string.Empty;
    
    public string Title {get; set; } = string.Empty;
    
    public string Description {get; set; } = string.Empty;
    
    public IncidentType IncidentType {get; set; }
    
    public int ClimbingAreaId {get; set; }

    public int? RouteId {get; set; }

    public string? AssignedTo {get; set; }
}