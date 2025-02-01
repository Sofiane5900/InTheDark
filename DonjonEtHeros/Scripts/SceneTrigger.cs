using System;
using Godot;

public partial class SceneTrigger : Area2D
{
    [Export]
    private string SceneToLoad = "Foret"; // Nom de la scène à charger

    public override void _Ready()
    {
        // Assure-toi que l'Area2D est bien configuré pour surveiller les objets
        Monitoring = true; // Active la détection
        GD.Print("Trigger prêt !");
    }

    private void OnBodyEntered(Node body)
    {
        // Vérifie si c'est le joueur (CharacterBody2D)
        if (body is CharacterBody2D player)
        {
            GD.Print($"Player détecté : {body.Name}");

            // Charger la scène 'Foret' en téléchargeant la position du joueur
            GameManager.Instance?.LoadScene(SceneToLoad, player.Position);
        }
    }
}
