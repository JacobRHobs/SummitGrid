using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SummitGrid.Api.Models;
using SummitGrid.Infrastructure;
using SummitGrid.Core.Entities;
using Microsoft.AspNetCore.Authorization;

namespace SummitGrid.Api.Controllers;

[ApiController]
[Route("api/routes")]

public class RouteController: ControllerBase
{
    private readonly SummitGridDbContext _context;
    public RouteController(SummitGridDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var routes = await _context.Routes.ToListAsync();

        var response = routes.Select(route => new RouteResponse
        {
            Id = route.Id,
            Name = route.Name,
            Description = route.Description,
            Grade = route.Grade,
            Type = route.Type,
            Status = route.Status,
            ClimbingAreaId = route.ClimbingAreaId,
            CreatedAt = route.CreatedAt,
            UpdatedAt = route.UpdatedAt
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int Id)
    {
        var routes = await _context.Routes.FindAsync(Id);
        if(routes == null) return NotFound();

        var response = new RouteResponse
        {
            Id = routes.Id,
            Name = routes.Name,
            Description = routes.Description,
            Grade = routes.Grade,
            Type = routes.Type,
            Status = routes.Status,
            ClimbingAreaId = routes.ClimbingAreaId,
            CreatedAt = routes.CreatedAt,
            UpdatedAt = routes.UpdatedAt
        };

        return Ok(response);
    }

    [Authorize(Roles = "Operator,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CreateRouteRequest route)
    {
        var _route = new SummitGrid.Core.Entities.Route
        {
            Name = route.Name,
            Description = route.Description,
            Grade = route.Grade,
            Type = route.Type,
            Status = route.Status,
            ClimbingAreaId = route.ClimbingAreaId
        };

        _context.Routes.Add(_route);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), route);
    }
}