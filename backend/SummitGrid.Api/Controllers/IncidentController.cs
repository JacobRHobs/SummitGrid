using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SummitGrid.Api.Models;
using SummitGrid.Core.Entities;
using SummitGrid.Core.Models;
using SummitGrid.Core.Results;
using SummitGrid.Infrastructure;
using SummitGrid.Infrastructure.Services;

namespace SummitGrid.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/incidents")]

public class IncidentController: ControllerBase
{
    private readonly SummitGridDbContext _context;
    private readonly IncidentService _incidentService;
    public IncidentController(SummitGridDbContext context, IncidentService incidentService)
    {
        _context = context;
        _incidentService = incidentService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var incidents = await _context.Incidents.ToListAsync();

        var response = incidents.Select(incident => new IncidentResponse
        {
            Id = incident.Id,
            Title = incident.Title,
            Description = incident.Description,
            IncidentType = incident.IncidentType,
            IncidentStatus = incident.IncidentStatus,
            ClimbingAreaId = incident.ClimbingAreaId,
            RouteId = incident.RouteId,
            ReportedBy = incident.ReportedBy,
            AssignedTo = incident.AssignedTo,
            CreatedAt = incident.CreatedAt,
            UpdatedAt = incident.UpdatedAt
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetbyId(int Id)
    {
        var incidents = await _context.Incidents.FindAsync(Id);
        if(incidents == null) return NotFound();

        var response = new IncidentResponse
        {
            Id = incidents.Id,
            Title = incidents.Title,
            Description = incidents.Description,
            IncidentType = incidents.IncidentType,
            IncidentStatus = incidents.IncidentStatus,
            ClimbingAreaId = incidents.ClimbingAreaId,
            RouteId = incidents.RouteId,
            ReportedBy = incidents.ReportedBy,
            AssignedTo = incidents.AssignedTo,
            CreatedAt = incidents.CreatedAt,
            UpdatedAt = incidents.UpdatedAt
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIncidentRequest incident)
    {
        if(string.IsNullOrEmpty(incident.Title))
            return BadRequest("Title is required.");
        
        if(string.IsNullOrEmpty(incident.Description))
            return BadRequest("Description is required.");
        
        CreateIncidentCommand _incident = new CreateIncidentCommand{
            Title = incident.Title,
            Description = incident.Description,
            IncidentType = incident.IncidentType,
            ClimbingAreaId = incident.ClimbingAreaId,
            RouteId = incident.RouteId,
            AssignedTo = incident.AssignedTo,
            UserName = User.Identity!.Name!
        };

        var success = await _incidentService.CreateIncidentAsync(_incident);
        if(!success) return BadRequest("Failed to create incident.");
        return CreatedAtAction(nameof(GetAll), null);
    }

    [Authorize(Roles = "Operator,Admin")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateIncidentStatusRequest request)
    {
        var success = await _incidentService.UpdateIncidentStatusAsync(id, request.Status, User.Identity!.Name!);
        if(success == UpdateIncidentResult.IncidentNotFound) return NotFound("Incident not found.");
        else if(success == UpdateIncidentResult.InvalidTransition) return BadRequest("Invalid status transition.");
        return Ok();
    }
}