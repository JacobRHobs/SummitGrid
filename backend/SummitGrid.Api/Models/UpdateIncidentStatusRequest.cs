using SummitGrid.Core.Entities;

namespace SummitGrid.Api.Models;

public class UpdateIncidentStatusRequest
{
    public IncidentStatus Status { get; set;}
}