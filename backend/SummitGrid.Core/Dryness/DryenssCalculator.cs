namespace SummitGrid.Core.Dryness;

public class DrynessCalculator
{
    private static readonly Dictionary<RockTypes, double> RockConverstion = new Dictionary<RockTypes, double>{
        { RockTypes.Granite, 1.0 },
        { RockTypes.Quartzite, 1.0 },
        { RockTypes.Slate, 0.91 },
        { RockTypes.Sandstone, 0.25 },
        { RockTypes.Limestone, 0.50 },
        { RockTypes.Basalt, 0.83 },
        { RockTypes.Conglomerate, 0.40 },
        { RockTypes.Gneiss, 1.0 },
    };

    public static double Calculate(List<WeatherReading> list, RockTypes type)
    {
        double Wetness = 0;

        if(RockConverstion.TryGetValue(type, out double multiplier)){
            foreach(WeatherReading w in list)
            {
                double temp = Wetness + w.Precipitation - (w.Et0 * multiplier);
                if(temp < 0)
                    Wetness = 0;
                else if(temp > 100)
                    Wetness = 100;
                else
                    Wetness = temp; 
            }
        }
        else{
            throw new ArgumentException($"Unknown rock type: {type}");
        } 

        return Wetness;
    }
}