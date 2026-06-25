using SummitGrid.Core.Dryness;

namespace SummitGrid.Infrastructure.Services;

public class DrynessService: IDrynessService
{
    public async Task<string> GetDrynessAsync(double lat, double lon)
    {
        return "Dry";
    }
}