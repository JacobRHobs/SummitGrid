using SummitGrid.Core.Entities;

namespace SummitGrid.Api.Models;
public class RouteResponse
{
    public int Id {get; set; }

    public string Name {get; set; } = string.Empty;

    public string? Description {get; set; }

    public string? Grade {get; set;}

    public RouteType Type {get; set; }

    public RouteStatus Status {get; set; }

    public int ClimbingAreaId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

}