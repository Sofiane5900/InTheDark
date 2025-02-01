using System;
using Godot;

public partial class SceneTrigger : Area2D
{
    [Export]
    private string SceneToLoad = "";

    public override void _Ready()
    {
        Monitoring = true;
        GD.Print("Trigger prêt !");

        // Connexion du signal _on_body_entered
        BodyEntered += _on_body_entered;
    }

    // Méthode appelée lorsqu'un Node2D entre dans la zone de collision
    public void _on_body_entered(Node2D body)
    {
        if (body is CharacterBody2D)
        {
            GD.Print($"Collision détectée avec {body.Name} !");
        }

        // Charge la scène quand il ne reste plus de tâches en attente
        CallDeferred(nameof(DeferredLoadScene));
    }

    private void DeferredLoadScene()
    {
        GameManager.Instance.LoadScene(SceneToLoad);
    }
}
