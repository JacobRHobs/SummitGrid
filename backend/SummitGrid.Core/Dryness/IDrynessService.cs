namespace SummitGrid.Core.Dryness;

public interface IDrynessService
{
    Task<string> GetDrynessAsync(double lat, double lon);
}