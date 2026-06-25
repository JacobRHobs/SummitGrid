using SummitGrid.Core.Entities;

namespace SummitGrid.Api.Models;

public class CreateClimbingAreaRequest
{
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string? AccessNotes { get; set; }

    public AreaStatus Status { get; set; }

    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
     
}