using System;
using Godot;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    private Node currentScene;

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

        // Map de départ
        LoadScene("Valombre");
    }

    public void LoadScene(string sceneName)
    {
        // Si une scene est déjà chargée, on la supprime
        if (currentScene != null)
        {
            currentScene.QueueFree();
        }

        // Nouvelle scene
        PackedScene newScene = GD.Load<PackedScene>($"res://Scenes/{sceneName}.tscn");
        if (newScene != null)
        {
            currentScene = newScene.Instantiate();
            AddChild(currentScene); // Ajout en tant qu'enfant de GameManager
            GD.Print($"Loading scene : {sceneName}");
        }
        else
        {
            GD.PrintErr($"Erreur loading: {sceneName}");
        }
    }
}
