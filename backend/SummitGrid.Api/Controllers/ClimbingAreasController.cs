using Microsoft.AspNetCore.Mvc;
using SummitGrid.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SummitGrid.Core.Entities;
using SummitGrid.Api.Models;
using NetTopologySuite.Geometries;
using Microsoft.AspNetCore.Authorization;
using SummitGrid.Core.Models;

namespace SummitGrid.Api.Controllers;

[ApiController]
[Route("api/climbingareas")]
public class ClimbingAreasController: ControllerBase
{
    private readonly SummitGridDbContext _context;
    public ClimbingAreasController(SummitGridDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var areas = await _context.ClimbingAreas.Include(a => a.Incidents).ToListAsync();

        var response = areas.Select(area => new ClimbingAreaResponse
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
            }).ToList()
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetbyId(int Id)
    {
        var areas = await _context.ClimbingAreas.Include(a => a.Incidents).FirstOrDefaultAsync(a => a.Id == Id);
        if(areas == null) return NotFound();

        var response = new ClimbingAreaResponse
        {
            Id = areas.Id,
            Name = areas.Name,
            Description = areas.Description,
            AccessNotes = areas.AccessNotes,
            Status = areas.Status,
            Latitude = areas.Location.Y,
            Longitude = areas.Location.X,
            CreatedAt = areas.CreatedAt,
            UpdatedAt = areas.UpdatedAt,
            Incidents = areas.Incidents.Select(i => new IncidentSummary
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                IncidentType = i.IncidentType,
                IncidentStatus = i.IncidentStatus
            }).ToList()
        };

        return Ok(response);
    }

    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearby(double lat, double lon, double radiusMiles)
    {
        var searchPoint = new Point(lon, lat) { SRID = 4326 };
        var radiusMeters = radiusMiles * 1609.34;
        
        var areas = await _context.ClimbingAreas.Include(a => a.Incidents).Where(a => a.Location.Distance(searchPoint) <= radiusMeters).ToListAsync();

        var response = areas.Select(area => new ClimbingAreaResponse
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
            }).ToList()
        }).ToList();

        return Ok(response);
    }

    [Authorize(Roles = "Operator,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClimbingAreaRequest area)
    {
        if(string.IsNullOrEmpty(area.Name))
            return BadRequest("Name is required.");
        
        ClimbingArea _area = new ClimbingArea
        {
            Name = area.Name,
            Description = area.Description,
            AccessNotes = area.AccessNotes,
            Status = area.Status,
            Location = new Point(area.Longitude, area.Latitude) { SRID = 4326 }
        };

        _context.ClimbingAreas.Add(_area);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), area);
    }
}  