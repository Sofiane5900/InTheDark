using System.Collections.Generic;
using Godot;

public partial class Map : Node2D
{
    [Export]
    public bool CanTriggerBattles { get; set; }

    [Export]
    public Godot.Collections.Array<string> PossibleEnemies { get; set; } =
        new Godot.Collections.Array<string>();

    public override void _Ready()
    {
        GD.Print($"🗺️ Map chargée: {Name}, CanTriggerBattles: {CanTriggerBattles}");
    }
}
