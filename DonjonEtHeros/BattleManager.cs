using System.Linq;
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
        var valombreNode = gameNode.GetNodeOrNull<Node>("Valombre");
        GD.Print(valombreNode); // Vérifie si le nœud existe ou non
        CharacterBody2D player = valombreNode.GetNodeOrNull<CharacterBody2D>("Character2D");
        GD.Print(player); // Vérifie si le joueur est bien trouvé

        previousSceneName = "Valombre";
        previousPlayerPosition = player.Position;
        (gameNode as GameManager)?.SaveSceneState(previousSceneName, previousPlayerPosition);
        // On désactive la camera du joueur
        Camera2D playerCamera = player.GetNode<Camera2D>("Camera2D");
        playerCamera.Enabled = false;

        // Instance de notre BattleMap
        Battle battleMap = battleScene.Instantiate<Battle>();
        battleMap.EnemyResource = enemy;

        // Activation de la camera BattleMap
        Camera2D battleCamera = battleMap.GetNode<Camera2D>("Camera2D");
        battleCamera.Enabled = true;

        if (valombreNode != null)
        {
            valombreNode.SetProcess(false); // Process = false
            GD.Print("Valombre scene en pause");
        }

        // Ajout de la BattleMap a GameManager
        gameNode.AddChild(battleMap);
        battleMap.Name = "BattleMap";
        GD.Print($"battleMap: {battleMap.Name}, enemy : {enemy}");

        // fix voidTask error, on a besoin de retourner une Task
        await Task.Delay(1000);
    }

    public async void EndBattle()
    {
        if (gameNode is null)
        {
            GD.PrintErr("EndBattle() - GameManager Node introuvable");
            return;
        }

        // Supprimer BattleMap si elle existe
        Node battleMap = gameNode.GetNodeOrNull("BattleMap");
        if (battleMap != null)
        {
            battleMap.QueueFree(); // Supprime complètement BattleMap
            GD.Print("BattleMap supprimé !");

            // Supprimer la caméra BattleMap
            Camera2D battleCamera = battleMap.GetNodeOrNull<Camera2D>("Camera2D");
            if (battleCamera != null)
            {
                battleCamera.Enabled = false;
            }
            else
            {
                GD.PrintErr("Battle camera is null");
            }
        }

        // On attend une frame avant d'éxécuter le reste du code
        await ToSignal(GetTree(), "process_frame");

        // On récupère la scène restaurée en cherchant l'enfant de GameNode (qui est une Node)
        Node activeScene = gameNode.GetChildren().FirstOrDefault(scene => scene is Node);
        if (activeScene == null)
        {
            GD.PrintErr("Impossible de retrouver la scène restaurée !");
            return;
        }

        GD.Print($"Nouvelle scène restaurée détectée : {activeScene.Name}");

        // Chercher Character2D dans cette scène
        CharacterBody2D player = activeScene.GetNodeOrNull<CharacterBody2D>("Character2D");
        if (player == null)
        {
            GD.PrintErr("Character2D introuvable dans la scène restaurée !");
        }
        else
        {
            GD.Print("Character2D trouvé après restauration !");
            player.Position = previousPlayerPosition;

            // Récupération de la caméra du joueur
            Camera2D playerCamera = player.GetNodeOrNull<Camera2D>("Camera2D");
            if (playerCamera != null)
            {
                playerCamera.Enabled = true;
            }
        }
    }
}
