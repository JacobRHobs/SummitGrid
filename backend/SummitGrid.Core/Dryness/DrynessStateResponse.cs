namespace SummitGrid.Core.Dryness;

public class DrynessStateResponse{
    public DrynessState DrynessState {get; set; }

    public RockTypes RockType {get; set; }

    public bool RockTypeFromDatabase {get; set;}
}