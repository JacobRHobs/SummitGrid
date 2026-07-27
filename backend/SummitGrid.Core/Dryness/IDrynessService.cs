namespace SummitGrid.Core.Dryness;

public interface IDrynessService
{
    Task<double> GetDrynessAsync(double lat, double lon);
}