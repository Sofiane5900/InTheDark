using System;
using System.Threading.Tasks;
using Godot;

public partial class BattleManager : Node
{
    public static BattleManager Instance { get; private set; }
    private Node gameNode;
    public BaseEnemy CurrentEnemy { get; private set; }

    public override void _Ready()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
            return;
        }
        gameNode = GetTree().Root.GetNode("Valombre") as Node;
    }

    public static async Task StartBattle(string battleScene, string enemyName)
    {
        PackedScene battleScenePath = GD.Load<PackedScene>($"res://Scenes/{battleScene}.tscn");
        if (battleScenePath is null)
        {
            GD.PrintErr($"Battle scene {battleScene} not found");
        }
        BaseEnemy enemyPath = GD.Load<BaseEnemy>($"res://Ressources/{enemyName}.tres");
        if (enemyPath is null)
        {
            GD.PrintErr($"Enemy {enemyName} not found");
        }

        await Instance?.StartBattleInstance(battleScenePath, enemyPath);
    }

    private async Task StartBattleInstance(PackedScene battleScene, BaseEnemy enemy)
    {
        // focus on this later
        // PauseOverworld();

        Battle battleMap = battleScene.Instantiate<Battle>();
        gameNode.AddChild(battleMap);
        battleMap.Name = "BattleMap";
        GD.Print($"battleMap: {battleMap.Name}, battleScene: {battleScene.ResourceName}");
    }
}
