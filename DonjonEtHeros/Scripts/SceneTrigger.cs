using System;
using Godot;

public partial class SceneTrigger : Area2D
{
    [Export]
    private string SceneToLoad = "Foret";

    public override void _Ready()
    {
        Monitoring = true;
        GD.Print("Trigger prêt !");

        // Connexion correcte en C#
        BodyEntered += _on_body_entered;
    }

    public void _on_body_entered(Node2D body)
    {
        if (body is CharacterBody2D)
        {
            GD.Print($"Collision détectée avec {body.Name} !");
        }

        // Charge la scène suivante
        GameManager.Instance.LoadScene(SceneToLoad);
    }
}
