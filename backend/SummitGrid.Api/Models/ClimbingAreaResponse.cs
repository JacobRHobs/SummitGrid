using SummitGrid.Core.Entities;
using SummitGrid.Core.Models;


namespace SummitGrid.Api.Models;

public class ClimbingAreaResponse
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string? AccessNotes { get; set; }
    
    public AreaStatus Status { get; set; }

    public double Latitude { get; set; }
    
    public double Longitude { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; } 
    
    public List<IncidentSummary>? Incidents { get; set; } = new List<IncidentSummary>();

    public List<RouteSummary>? Routes { get; set; } = new List<RouteSummary>();
}