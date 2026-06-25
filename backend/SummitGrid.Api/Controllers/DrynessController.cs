using Microsoft.AspNetCore.Mvc;
using SummitGrid.Core.Dryness;

namespace SummitGrid.Api.Controllers;

[ApiController]
[Route("api/dryness")]
public class DrynessController: ControllerBase
{
    private readonly IDrynessService _drynessService;

    public DrynessController(IDrynessService drynessService)
    {
        _drynessService = drynessService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get(double lat, double lon)
    {
        var response = await _drynessService.GetDrynessAsync(lat, lon);
        return Ok(response);
    }
}