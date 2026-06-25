using SummitGrid.Core.Entities;

namespace SummitGrid.Api.Models;

public class CreateRouteRequest
{
    public string Name {get; set; } = string.Empty;

    public string? Description {get; set; }

    public string? Grade {get; set;}

    public RouteType Type {get; set; }

    public RouteStatus Status {get; set; }

    public int ClimbingAreaId { get; set; }
}