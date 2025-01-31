using System;
using Godot;

// TODO : Rendre le retour à la scène précédente dynamique, en fonction de la scène actuelle
// ? Il y a toujours des réferences a "Valombre" dans le code
public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    private Node currentScene;
    private string previousSceneName;

    private Vector2? previousCharacterPosition;

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
                CharacterBody2D player = currentScene.GetNodeOrNull<CharacterBody2D>(
                    "Valombre/Character2D"
                );
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

    // public void LoadPreviousScene()
    // {
    //     GD.Print($"LoadPreviousScene(): {previousSceneName}");
    //     // Vérification si le nom de scène est valide
    //     if (!string.IsNullOrEmpty(previousSceneName))
    //     {
    //         LoadScene(previousSceneName, previousCharacterPosition);
    //     }
    //     else
    //     {
    //         GD.PrintErr("Aucune scène précédente à charger");
    //     }
    // }

    private void CheckForPlayer()
    {
        // Vérifie que "Character2D" existe dans la scène
        CharacterBody2D player = currentScene.GetNodeOrNull<CharacterBody2D>("Character2D");
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
