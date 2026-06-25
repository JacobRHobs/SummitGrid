using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SummitGrid.Api.Models;
using SummitGrid.Core.Entities;
using SummitGrid.Infrastructure;
using SummitGrid.Core.Models;

namespace SummitGrid.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/dashboard")]
public class DashboardController: ControllerBase
{
    private readonly SummitGridDbContext _context;
    public DashboardController(SummitGridDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetDashboard(double lat, double lon, double radiusMiles)
    {
        var searchPoint = new Point(lon, lat) { SRID = 4326 };
        var radiusMeters = radiusMiles * 1609.34;
        
        var areas = await _context.ClimbingAreas.Include(a => a.Incidents).Include(a => a.Routes).Where(a => a.Location.Distance(searchPoint) <= radiusMeters).ToListAsync();
        var areaResponses = areas.Select(area => new ClimbingAreaResponse
        {
            Id = area.Id,
            Name = area.Name,
            Description = area.Description,
            AccessNotes = area.AccessNotes,
            Status = area.Status,
            Latitude = area.Location.Y,
            Longitude = area.Location.X,
            CreatedAt = area.CreatedAt,
            UpdatedAt = area.UpdatedAt,
            Incidents = area.Incidents.Select(i => new IncidentSummary
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                IncidentType = i.IncidentType,
                IncidentStatus = i.IncidentStatus
            }).ToList(),
            Routes = area.Routes.Select(a => new RouteSummary
            {
                Id = a.Id,
                Name = a.Name,
                Grade = a.Grade,
                Type = a.Type,
                Status = a.Status
            }).ToList()
        }).ToList();
        
        var allIncidents = areas.SelectMany(a => a.Incidents).ToList();
        var response = new DashboardResponse
        {
            TotalActiveIncidents = allIncidents.Count(i => 
                i.IncidentStatus == IncidentStatus.Reported ||
                i.IncidentStatus == IncidentStatus.Acknowledged ||
                i.IncidentStatus == IncidentStatus.InProgress),
            OpenAreas = areas.Count(a => a.Status == AreaStatus.Open),
            RestrictedAreas = areas.Count(a => a.Status == AreaStatus.Restricted),
            ClosedAreas = areas.Count(a => a.Status == AreaStatus.Closed),
            ReportedIncidents = allIncidents.Count(i => i.IncidentStatus == IncidentStatus.Reported),
            AcknowledgedIncidents = allIncidents.Count(i => i.IncidentStatus == IncidentStatus.Acknowledged),
            InProgressIncidents = allIncidents.Count(i => i.IncidentStatus == IncidentStatus.InProgress),
            ClimbingAreas = areaResponses
        };

        return Ok(response);
    }
}