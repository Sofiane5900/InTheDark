using System.Linq;
using System.Threading.Tasks;
using Godot;

// ! Parfois, la previousScene n'est pas "paused" correctement
// TODO : Pause du jeu lors du combat, et reprise après le combat
public partial class BattleManager : Node
{
    public static BattleManager Instance { get; private set; }
    private Node gameNode; // Reference a ma Node GameManager
    public BaseEnemy CurrentEnemy { get; private set; }

    private Node previousSceneName;
    private Vector2 previousPlayerPosition;

    public override void _EnterTree()
    {
        // Singleton
        if (Instance is null)
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
        // Récupérer la scène active actuelle
        Node currentScene = gameNode.GetChildren().FirstOrDefault(scene => scene is Node);
        if (currentScene is null)
        {
            GD.PrintErr("Aucune scène active trouvée avant le combat !");
            return;
        }

        // Récupérer le joueur
        CharacterBody2D player =
            currentScene.FindChild("Character2D", true, false) as CharacterBody2D;
        if (player is null)
        {
            GD.PrintErr("Impossible de trouver Character2D avant le combat !");
            return;
        }

        // Sauvegarde de l'état de la scène avant combat
        previousPlayerPosition = player.Position;
        (gameNode as GameManager).SaveSceneState(
            // On récupere le nom de la scéne via le fichier
            currentScene.SceneFilePath.GetFile().TrimSuffix(".tscn"),
            previousPlayerPosition
        );

        // On désctive la caméra du joueur
        Camera2D playerCamera = player.GetNodeOrNull<Camera2D>("Camera2D");
        if (playerCamera is not null)
        {
            playerCamera.Enabled = false;
        }

        // ! ça marche une fois sur trois :(
        // Pause de la scéne active
        currentScene.SetProcess(false);
        GD.Print($"{currentScene.Name} mise en pause");

        // Instancier la BattleMap avec enemy.tres
        Battle battleMap = battleScene.Instantiate<Battle>();
        battleMap.EnemyResource = enemy;
        gameNode.AddChild(battleMap);
        battleMap.Name = "BattleMap";

        // On active la cémera de la battleMap
        Camera2D battleCamera = battleMap.GetNodeOrNull<Camera2D>("Camera2D");
        if (battleCamera is not null)
        {
            battleCamera.Enabled = true;
        }

        GD.Print($"Combat commencé sur {battleMap.Name} contre {enemy}");

        await Task.Delay(1000); // J'ai rajouté ça car il faut return une Task, (fix void task blabla)
    }

    public async void EndBattle()
    {
        if (gameNode is null)
        {
            GD.PrintErr("EndBattle() - GameManager Node introuvable");
            return;
        }

        // On récupere la BattleMap
        Node battleMap = gameNode.GetNodeOrNull("BattleMap");
        // On défini ce qu'est le joueur (un enfant d'une scéne)
        CharacterBody2D player = FindChild("Character2D") as CharacterBody2D;

        // On ajoute le joueur a la scéne
        if (player is not null)
        {
            gameNode.AddChild(player);
        }

        // Supression de la BattleMap (car combat fini)
        if (battleMap is not null)
        {
            battleMap.QueueFree();
            GD.Print("BattleMap supprimée !");
        }

        // Attendre une frame pour érte sur que la BattleMap est bien supprimée
        await ToSignal(GetTree(), "process_frame");

        // Reload the previous scene
        (gameNode as GameManager)?.LoadPreviousScene();

        // Attendre une frame pour étre sur que la scéne est bien chargée
        await ToSignal(GetTree(), "process_frame");

        // On récupere la scéne restaurée après le combat, FirstOrDefault requéte LINQ pour trouver le premier élément qui est une Node
        Node restoredScene = gameNode.GetChildren().FirstOrDefault(scene => scene is Node);
        if (restoredScene is null)
        {
            GD.PrintErr("EndBattle() - Impossible de restaurer la scène !");
            return;
        }

        // On restaure la position du joueur,
        player = restoredScene.FindChild("Character2D") as CharacterBody2D;
        if (player is null)
        {
            GD.PrintErr("EndBattle() - Character2D introuvable après restauration !");
            return;
        }

        player.Position = previousPlayerPosition;

        // On re-active la caméra du joueur
        Camera2D playerCamera = player.GetNodeOrNull<Camera2D>("Camera2D");
        if (playerCamera is not null)
        {
            playerCamera.Enabled = true;
        }

        // Et on enleve la pause de la scéne restaurée (l'ancienne currentScene)
        restoredScene.SetProcess(true);
        GD.Print($"{restoredScene.Name} réactivée après combat !");
    }
}
