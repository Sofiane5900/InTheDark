using System;
using Godot;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    private Node currentScene;
    private string previousSceneName;
    private Vector2? previousCharacterPosition;

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
            return;
        }

        // Scéne de depart
        LoadScene("Valombre");
    }

    public void LoadScene(string sceneName, Vector2? playerPosition = null) // Position de départ en null pour éviter erreur
    {
        if (currentScene != null)
        {
            currentScene.QueueFree(); // On supprime la scène actuelle quand on en charge une nouvelle
        }

        PackedScene newScene = GD.Load<PackedScene>($"res://Scenes/{sceneName}.tscn");
        if (newScene != null)
        {
            currentScene = newScene.Instantiate();
            AddChild(currentScene);
            GD.Print($"Nouvelle scene : {sceneName}");

            // Restaure la position du joueur si elle n'est pas null
            CharacterBody2D player = currentScene.GetNode<CharacterBody2D>("Character2D");
            if (player is not null && playerPosition.HasValue)
            {
                player.Position = playerPosition.Value;
                GD.Print($"Position du joueur restauré : {player.Position}");
            }
        }
        else
        {
            GD.PrintErr($"Impossible de charger la scéne: {sceneName}");
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
        // Verification string is not null or empty
        if (!string.IsNullOrEmpty(previousSceneName))
        {
            LoadScene(previousSceneName, previousCharacterPosition);
        }
        else
        {
            GD.PrintErr("Aucune scéne précédente à charger");
        }
    }

    private void PauseScene()
    {
        GetTree().Paused = true;
    }

    private void ResumeScene()
    {
        GetTree().Paused = false;
    }
}
