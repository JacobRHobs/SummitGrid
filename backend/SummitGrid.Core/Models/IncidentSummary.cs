using SummitGrid.Core.Entities;

namespace SummitGrid.Core.Models;
public class IncidentSummary
{
    public int Id {get; set; }
    public string Title {get; set; } = string.Empty;
    
    public string Description {get; set; } = string.Empty;
    
    public IncidentType IncidentType {get; set; }
    
    public IncidentStatus IncidentStatus {get; set; }
}