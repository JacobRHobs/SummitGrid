using System.Text.Json;
using SummitGrid.Core.Dryness;

namespace SummitGrid.Infrastructure.Services;

public class DrynessService: IDrynessService
{
    private readonly HttpClient _httpClient;

    public DrynessService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<double> GetDrynessAsync(double lat, double lon)
    {
        string apiCall = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&hourly=et0_fao_evapotranspiration,rain,showers&past_days=3";
        var response = await _httpClient.GetStringAsync(apiCall);
        var weatherData = JsonSerializer.Deserialize<WeatherForecastResponse>(response) ?? throw new Exception("Failed to deserialize weather data.");
        
        List<WeatherReading> weatherReadings = new List<WeatherReading>();

        for(int i = 0; i < weatherData.Hourly.Et0.Count; i++)
        {
            WeatherReading temp = new WeatherReading();

            temp.Precipitation = weatherData.Hourly.Rain[i] + weatherData.Hourly.Showers[i];
            temp.Et0 = weatherData.Hourly.Et0[i];

            weatherReadings.Add(temp);
        }

        var rockType = await GetRockTypesAsync(lat, lon);

        return DrynessCalculator.Calculate(weatherReadings, rockType);
    }

    // Macrostrat returns a stratigraphic column: multiple rock layers, each potentially containing multiple lithologies.
    // We iterate youngest-first and return the first lith name that maps to a known RockType.
    private async Task<RockTypes> GetRockTypesAsync(double lat, double lon)
    {
        string apiCallColumn = $"https://macrostrat.org/api/columns?lat={lat}&lng={lon}";
        var response = await _httpClient.GetStringAsync(apiCallColumn);
        var columnResponse = JsonSerializer.Deserialize<ColumnResponse>(response) ?? throw new Exception("Failed to deserialize column data.");
        var colId = columnResponse.Success.Data[0].ColId;

        string apiCallLiths = $"https://macrostrat.org/api/units?col_id={colId}&format=json&response=long";
        response = await _httpClient.GetStringAsync(apiCallLiths);
        var lithData = JsonSerializer.Deserialize<RockTypeResponse>(response) ?? throw new Exception("Failed to deserialize rock data.");

        foreach (RockTypeUnit x in lithData.Success.Data)
        {
            foreach (RockTypeLith lith in x.Lith)
            {
                if (Enum.TryParse<RockTypes>(lith.Name, true, out RockTypes rockType))
                    return rockType;
            }
        }

        //If no rock type found returns rock type with standard 1x multiplier
        return RockTypes.Granite;
    }
}