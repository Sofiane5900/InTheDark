using System;
using System.Threading.Tasks;
using Godot;

public partial class BattleManager : Node
{
    public static BattleManager Instance { get; private set; }
    private Node gameNode; // Reference a ma Node GameManager
    public BaseEnemy CurrentEnemy { get; private set; }

    public override void _Ready()
    {
        // Si l'instance n'existe pas, on la crée
        if (Instance is null)
        {
            Instance = this;
        }
        // Si une instance existe déjà, on la supprime (on veux qu'il n'y ai qu'une seule instance de BattleManager)
        else
        {
            QueueFree();
            return;
        }

        // On récupere la Node Root GameManager
        gameNode = GetTree().Root.GetNode("GameManager");
    }

    public static async Task StartBattle(string battleScene, string enemyName)
    {
        // Chemin map battle
        PackedScene battleScenePath = GD.Load<PackedScene>($"res://Scenes/{battleScene}.tscn");
        if (battleScenePath is null)
        {
            GD.PrintErr($"Battle scene {battleScene} non-trouvé");
            return;
        }

        // Chemin de mon ennemy ressource
        BaseEnemy enemyPath = GD.Load<BaseEnemy>($"res://Ressources/{enemyName}.tres");
        if (enemyPath is null)
        {
            GD.PrintErr($"Enemy {enemyName} non-trouvé");
            return;
        }

        // On instancie un combat avec chemin de la map et chemin ressource ennemy
        await Instance?.StartBattleInstance(battleScenePath, enemyPath);
    }

    private async Task StartBattleInstance(PackedScene battleScene, BaseEnemy enemy)
    {
        GetTree().Paused = false;
        // Valombre est un enfant de GameManager
        CharacterBody2D player = gameNode.GetNode<CharacterBody2D>("Valombre/Character2D");

        // On désactive la camera du joueur
        Camera2D playerCamera = player.GetNode<Camera2D>("Camera2D");
        playerCamera.Enabled = false;

        // Instance de notre BattleMap
        Battle battleMap = battleScene.Instantiate<Battle>();

        // Activation de la camera BattleMap
        Camera2D battleCamera = battleMap.GetNode<Camera2D>("Camera2D");
        battleCamera.Enabled = true;

        // On supprime la précédente map "Valombre"
        var valombreNode = gameNode.GetNodeOrNull<Node>("Valombre");
        if (valombreNode != null)
        {
            valombreNode.QueueFree(); // Suppression de la node
        }

        // Ajout de la BattleMap a GameManager
        gameNode.AddChild(battleMap);
        battleMap.Name = "BattleMap";
        GD.Print($"battleMap: {battleMap.Name}, battleScene: {battleScene.ResourceName}");

        // fix voidTask error, on a besoin de retourner une Task
        await Task.Delay(1000);
    }
}
