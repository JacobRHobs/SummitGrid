using System.Text.Json.Serialization;

namespace SummitGrid.Core.Dryness;

public class RockTypeResponse
{
    [JsonPropertyName("success")]
    public RockTypeSuccess Success { get; set; } = new RockTypeSuccess();
}

public class RockTypeSuccess
{
    [JsonPropertyName("data")]
    public List<RockTypeUnit> Data {get; set;} = new List<RockTypeUnit>();
}

public class RockTypeUnit
{
    [JsonPropertyName("lith")]
    public List<RockTypeLith> Lith {get; set;} = new List<RockTypeLith>();
}

public class RockTypeLith
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class ColumnResponse
{
    [JsonPropertyName("success")]
    public ColumnSuccess Success {get; set; } = new ColumnSuccess();
}

public class ColumnSuccess
{
    [JsonPropertyName("data")]
    public List<ColumnData> Data { get; set; }= new List<ColumnData>();
}

public class ColumnData
{
    [JsonPropertyName("col_id")]
    public int ColId { get; set; }
}