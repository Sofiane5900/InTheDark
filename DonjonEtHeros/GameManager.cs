using System;
using Godot;

// TODO : Rendre le retour à la scène précédente dynamique, en fonction de la scène actuelle
// ? Il y a toujours des réferences a "Valombre" dans le code
public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    private Node currentScene;
    private Map currentMap;
    private string previousSceneName;
    private CharacterBody2D player;

    private Vector2? previousCharacterPosition;

    private float encounterChance = 0f; // "Chance" accumulée

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

        // Scène de départ
        LoadScene("Valombre");
    }

    public override void _Process(double delta)
    {
        if (currentMap is null || !currentMap.CanTriggerBattles || player is null)
        {
            return; // Pas de combat car une des conditions n'est pas remplie
        }

        // On vérifie si le joueur bouge
        if (player.Velocity.Length() > 0)
        {
            encounterChance += (float)delta * 10;

            // Si la chance dépasse ou égale 100, on déclenche un combat
            if (encounterChance >= 100)
            {
                StartRandomBattle();
                encounterChance = 0; // Reset après le combat
            }
        }
    }

    private async void StartRandomBattle()
    {
        if (currentMap.PossibleEnemies.Count == 0)
            return; // Pas d'ennemis ici

        RandomNumberGenerator rng = new RandomNumberGenerator();
        int index = rng.RandiRange(0, currentMap.PossibleEnemies.Count - 1);
        string enemyName = currentMap.PossibleEnemies[index];

        GD.Print($"Combat déclenché avec {enemyName} !");

        await BattleManager.StartBattle("BattleMap", enemyName);
    }

    public void LoadScene(string sceneName, Vector2? playerPosition = null)
    {
        if (currentScene is not null)
        {
            currentScene.QueueFree(); // Supprime la scène actuelle
        }

        PackedScene newScene = GD.Load<PackedScene>($"res://Scenes/{sceneName}.tscn");
        if (newScene is not null)
        {
            currentScene = newScene.Instantiate();
            AddChild(currentScene);
            GD.Print($"Nouvelle scène : {sceneName}");
            currentMap = currentScene.FindChild("Map") as Map;

            // Affiche les nœuds enfants de la scène pour déboguer
            foreach (Node child in currentScene.GetChildren())
            {
                GD.Print($"Enfant trouvé : {child.Name}, chemin : {child.GetPath()}");
            }

            // Déférer la recherche du joueur pour garantir que la scène est prête
            CallDeferred(nameof(CheckForPlayer));

            // Restaure la position du joueur si elle n'est pas null
            if (playerPosition.HasValue)
            {
                player = currentScene.FindChild("Character2D", true, false) as CharacterBody2D;
                if (player is not null)
                {
                    player.Position = playerPosition.Value;
                    GD.Print($"Position du joueur restaurée : {player.Position}");
                }
                else
                {
                    GD.PrintErr(
                        "Character2D n'est pas trouvé dans la scène après la restauration."
                    );
                }
            }
        }
        else
        {
            GD.PrintErr($"Impossible de charger la scène : {sceneName}");
        }
    }

    public void SaveSceneState(string sceneName, Vector2 characterPosition)
    {
        previousSceneName = sceneName;
        previousCharacterPosition = characterPosition;
        GD.Print($"Sauvegarde: {sceneName}, position: {characterPosition}");
    }

    public void LoadPreviousScene()
    {
        GD.Print($"LoadPreviousScene(): {previousSceneName}");
        // Vérification si le nom de scène est valide
        if (!string.IsNullOrEmpty(previousSceneName))
        {
            LoadScene(previousSceneName, previousCharacterPosition);
        }
        else
        {
            GD.PrintErr("Aucune scène précédente à charger");
        }
    }

    private void CheckForPlayer()
    {
        // Vérifie que "Character2D" existe dans la scène
        player = currentScene.FindChild("Character2D", true, false) as CharacterBody2D;
        if (player is null)
        {
            GD.PrintErr("Character2D n'est toujours pas trouvé dans la scène !");
        }
        else
        {
            GD.Print("Character2D trouvé !");
        }
    }
}
