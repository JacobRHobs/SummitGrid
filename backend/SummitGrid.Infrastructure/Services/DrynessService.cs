using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using SummitGrid.Core.Dryness;

namespace SummitGrid.Infrastructure.Services;



public class DrynessService: IDrynessService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;

    public DrynessService(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }
    
    public async Task<DrynessStateResponse> GetDrynessAsync(double lat, double lon, RockTypes? rockType)
    {
        var weatherData = new WeatherForecastResponse();
        string cacheKey = $"{Math.Round(lat, 1)},{Math.Round(lon, 1)}";
        
        //If/else block caches weather values based on the nearest lat/lon to the nearest .1 decimal place. If not cached, make call to OpenMateo
        if(_cache.TryGetValue<WeatherForecastResponse>(cacheKey, out  weatherData)){}

        else{
            string apiCall = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&hourly=et0_fao_evapotranspiration,rain,showers&past_days=3";
            var response = await _httpClient.GetStringAsync(apiCall);
            weatherData = JsonSerializer.Deserialize<WeatherForecastResponse>(response) ?? throw new Exception("Failed to deserialize weather data.");
            _cache.Set(cacheKey, weatherData, TimeSpan.FromHours(1));
        }

        if (weatherData == null) throw new Exception("Weather data unavailable.");
        
        List<WeatherReading> weatherReadings = new List<WeatherReading>();

        // Combine Rain and Showers from OpenMateo into a single Precipitation value per hour for the calculator
        for(int i = 0; i < weatherData.Hourly.Et0.Count; i++)
        {
            WeatherReading temp = new WeatherReading();

            temp.Precipitation = weatherData.Hourly.Rain[i] + weatherData.Hourly.Showers[i];
            temp.Et0 = weatherData.Hourly.Et0[i];

            weatherReadings.Add(temp);
        }

        bool fromDatabase = rockType != null;
        if(rockType == null)
            rockType = await GetRockTypesAsync(lat, lon);

        double dryness = DrynessCalculator.Calculate(weatherReadings, rockType.Value);
        
        DrynessStateResponse drynessStateResponse = new DrynessStateResponse 
        { 
            DrynessState = DrynessCalculator.GetState(dryness, rockType.Value),
            RockType = rockType.Value,
            RockTypeFromDatabase = fromDatabase
        };
        
        return drynessStateResponse;
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