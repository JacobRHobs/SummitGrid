namespace SummitGrid.Core.Dryness;

public interface IDrynessService
{
    Task<DrynessState> GetDrynessAsync(double lat, double lon);
}