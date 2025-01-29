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
        // Une seule instance de BattleManager doit exister
        if (Instance is null)
        {
            Instance = this;
        }
        // Si une instance existe déjà, on la détruit
        else
        {
            QueueFree();
            return;
        }

        gameNode = GetTree().Root.GetNode("Valombre");
    }

    public static async Task StartBattle(string battleScene, string enemyName)
    {
        PackedScene battleScenePath = GD.Load<PackedScene>($"res://Scenes/{battleScene}.tscn");
        if (battleScenePath is null)
        {
            GD.PrintErr($"Battle scene {battleScene} non trouvé");
        }
        BaseEnemy enemyPath = GD.Load<BaseEnemy>($"res://Ressources/{enemyName}.tres");
        if (enemyPath is null)
        {
            GD.PrintErr($"Enemy {enemyName} non trouvé");
        }

        // Je change la camera a celle de ma scène de

        await Instance?.StartBattleInstance(battleScenePath, enemyPath);
    }

    private async Task StartBattleInstance(PackedScene battleScene, BaseEnemy enemy)
    {
        Battle battleMap = battleScene.Instantiate<Battle>();
        gameNode.AddChild(battleMap);
        battleMap.Name = "BattleMap";
        GD.Print($"battleMap: {battleMap.Name}, battleScene: {battleScene.ResourceName}");
    }
}
