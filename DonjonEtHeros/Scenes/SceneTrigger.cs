using System;
using Godot;

public partial class SceneTransitionTrigger : Area2D
{
    [Export]
    public string SceneToLoad = "Forest"; // Scene à charger

    public override void _Ready()
    {
        Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
    }

    private void OnBodyEntered(Node body)
    {
        if (body is CharacterBody2D) // Vérifie que c'est le joueur
        {
            GD.Print($"Changement de scène vers {SceneToLoad}");
            (GetTree().Root.GetNode("GameManager") as GameManager)?.LoadScene(SceneToLoad);
        }
    }
}
