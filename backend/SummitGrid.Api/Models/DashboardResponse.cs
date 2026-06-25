using SummitGrid.Core.Entities;

namespace SummitGrid.Api.Models;

public class DashboardResponse
{
    public int TotalActiveIncidents {get; set; }

    public int OpenAreas {get; set; }

    public int RestrictedAreas {get; set; }

    public int ClosedAreas {get; set; }

    public int ReportedIncidents {get; set; }

    public int AcknowledgedIncidents {get; set; }

    public int InProgressIncidents {get; set; }

    public List<ClimbingAreaResponse> ClimbingAreas {get; set; } = new List<ClimbingAreaResponse>();
}