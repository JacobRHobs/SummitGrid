namespace SummitGrid.Core.Dryness;

public interface IDrynessService
{
    Task<DrynessStateResponse> GetDrynessAsync(double lat, double lon, RockTypes? rockType);
}