using Microsoft.AspNetCore.Mvc;
using SummitGrid.Core.Dryness;
using SummitGrid.Core.Entities;
using SummitGrid.Infrastructure;

namespace SummitGrid.Api.Controllers;

[ApiController]
[Route("api/dryness")]
public class DrynessController: ControllerBase
{
    private readonly IDrynessService _drynessService;

    private readonly SummitGridDbContext _context;

    public DrynessController(IDrynessService drynessService, SummitGridDbContext context)
    {
        _drynessService = drynessService;
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get(int areaId)
    {
        var climbingArea = await _context.ClimbingAreas.FindAsync(areaId);
        if(climbingArea == null) return NotFound($"Area {areaId} not found");

        var response = await _drynessService.GetDrynessAsync(climbingArea.Location.Y, climbingArea.Location.X, climbingArea.RockType);
        
        //Update db with climbing area's rock type if it didn't already exist
        if (!response.RockTypeFromDatabase)
        {
            climbingArea.RockType = response.RockType;
            await _context.SaveChangesAsync();
        }


        return Ok(response);
    }
}