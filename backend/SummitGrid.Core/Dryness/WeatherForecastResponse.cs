using System.Text.Json.Serialization;

namespace SummitGrid.Core.Dryness;

public class WeatherForecastResponse
{
    [JsonPropertyName("hourly")]
    public HourlyData Hourly { get; set; } = new HourlyData();
}

public class HourlyData
{
    [JsonPropertyName("et0_fao_evapotranspiration")]
    public List<double> Et0 { get; set; } = new List<double>();

    [JsonPropertyName("rain")]
    public List<double> Rain { get; set; } = new List<double>();

    [JsonPropertyName("showers")]
    public List<double> Showers { get; set; } = new List<double>();
}