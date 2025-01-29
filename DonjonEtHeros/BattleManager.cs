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
        // Si aucune instance n'existe, on la crée
        if (Instance is null)
        {
            Instance = this;
        }
        // Sinons, si une instance existe déjà, on la détruit (on veux seulement une instance de BattleManager)
        else
        {
            QueueFree();
            return;
        }

        gameNode = GetTree().Root.GetNode("Valombre");
    }

    public static async Task StartBattle(string battleScene, string enemyName)
    {
        // Chemin de la scene de combat
        PackedScene battleScenePath = GD.Load<PackedScene>($"res://Scenes/{battleScene}.tscn");
        if (battleScenePath is null)
        {
            GD.PrintErr($"Battle scene {battleScene} not found");
            return;
        }

        // Chemin de l'ennemi
        BaseEnemy enemyPath = GD.Load<BaseEnemy>($"res://Ressources/{enemyName}.tres");
        if (enemyPath is null)
        {
            GD.PrintErr($"Enemy {enemyName} not found");
            return;
        }

        await Instance?.StartBattleInstance(battleScenePath, enemyPath);
    }

    private async Task StartBattleInstance(PackedScene battleScene, BaseEnemy enemy)
    {
        CharacterBody2D player = gameNode.GetNode<CharacterBody2D>("Character2D");

        // On désactive la camera du joueur
        Camera2D playerCamera = player.GetNode<Camera2D>("Camera2D");
        playerCamera.Enabled = false;

        // Instance de la map battle
        Battle battleMap = battleScene.Instantiate<Battle>();

        // Enable la camera de la map battle
        Camera2D battleCamera = battleMap.GetNode<Camera2D>("Camera2D");
        battleCamera.Enabled = true;

        // Ajouter la scene BattleMap a la scene principale en en tant qu'enfant
        gameNode.AddChild(battleMap);
        battleMap.Name = "BattleMap";
        GD.Print($"battleMap: {battleMap.Name}, battleScene: {battleScene.ResourceName}");

        // Ajout d'un task.delay pour eviter erreur voidTask
        await Task.Delay(1000);
    }
}
