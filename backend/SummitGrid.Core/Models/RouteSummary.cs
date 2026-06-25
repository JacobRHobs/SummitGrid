using SummitGrid.Core.Entities;

namespace SummitGrid.Core.Models;

public class RouteSummary
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public string? Grade {get; set;}

    public RouteType Type {get; set; }

    public RouteStatus Status {get; set; } = RouteStatus.Open;
}