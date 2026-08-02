using NetTopologySuite.Geometries;
using SummitGrid.Core.Dryness;

namespace SummitGrid.Core.Entities;
public class ClimbingArea
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string? AccessNotes { get; set; }
    
    public AreaStatus Status { get; set; } = AreaStatus.Open;
    
    public RockTypes? RockType {get; set; } 

    public Point Location { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Incident> Incidents { get; set; } = new List<Incident>();

    public ICollection<Route> Routes { get; set; } = new List<Route>();
}