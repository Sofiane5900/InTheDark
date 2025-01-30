using System.Threading.Tasks;
using Godot;

// TODO : Rendre le retour à la scène précédente dynamique, en fonction de la scéne actuelle
public partial class BattleManager : Node
{
    public static BattleManager Instance { get; private set; }
    private Node gameNode; // Reference a ma Node GameManager
    public BaseEnemy CurrentEnemy { get; private set; }

    private string previousSceneName;
    private Vector2 previousPlayerPosition;

    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
        }
    }

    public override void _Ready()
    {
        gameNode = GetTree().Root.GetNodeOrNull("GameManager");
        if (gameNode is null)
        {
            GD.PrintErr("GameManager Node introuvable");
        }
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
        // Valombre est un enfant de GameManager
        CharacterBody2D player = gameNode.GetNode<CharacterBody2D>("Valombre/Character2D");

        previousSceneName = "Valombre";
        previousPlayerPosition = player.Position;
        (gameNode as GameManager)?.SaveSceneState(previousSceneName, previousPlayerPosition);
        // On désactive la camera du joueur
        Camera2D playerCamera = player.GetNode<Camera2D>("Camera2D");
        playerCamera.Enabled = false;

        // Instance de notre BattleMap
        Battle battleMap = battleScene.Instantiate<Battle>();

        // Activation de la camera BattleMap
        Camera2D battleCamera = battleMap.GetNode<Camera2D>("Camera2D");
        battleCamera.Enabled = true;

        var valombreNode = gameNode.GetNode<Node>("Valombre");
        if (valombreNode != null)
        {
            valombreNode.SetProcess(false); // Process = false
            GD.Print("Valombre scene en pause");
        }

        // Ajout de la BattleMap a GameManager
        gameNode.AddChild(battleMap);
        battleMap.Name = "BattleMap";
        GD.Print($"battleMap: {battleMap.Name}, battleScene: {battleScene.ResourceName}");

        // fix voidTask error, on a besoin de retourner une Task
        await Task.Delay(1000);
    }

    public void EndBattle()
    {
        if (gameNode is null)
        {
            GD.PrintErr("EndBattle() - GameManager Node introuvable");
            return;
        }

        Node battleMap = gameNode.GetNodeOrNull("BattleMap");
        if (battleMap != null)
        {
            battleMap.QueueFree(); // Supprime complètement BattleMap
            GD.Print("BattleMap supprimé !");
        }

        // Supression de la camera BattleMap
        Camera2D battleCamera = gameNode.GetNode<Camera2D>("BattleMap/Camera2D");
        if (battleCamera is null)
        {
            GD.PrintErr("Battle camera is null");
        }
        battleCamera.Enabled = false;

        // EndBattle() - Retour a Valombre
        (gameNode as GameManager)?.LoadPreviousScene();

        // Restauration de la position du joueur
        CharacterBody2D player = gameNode.GetNode<CharacterBody2D>("Valombre/Character2D");
        if (player is not null)
        {
            // Restauration de la camera du joueur
            Camera2D playerCamera = player.GetNode<Camera2D>("Camera2D");
            playerCamera.Enabled = true;
        }

        var valombreScene = gameNode.GetNode<Node>("Valombre");
        if (valombreScene is not null)
        {
            valombreScene.SetProcess(true); // Process = true
            GD.Print("Valombre scene reprend");
        }

        GD.Print("EndBattle() - Retour a Valombre");
    }
}
