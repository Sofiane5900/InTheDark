using Godot;

public partial class Barriere : Area2D
{
    public override void _Ready()
    {
        GD.Print("🚧 Barrière prête !");
        BodyEntered += _on_body_entered;
    }

    private void _on_body_entered(Node body)
    {
        GD.Print($"📌 Détection : {body.Name}");

        if (body is Character player)
        {
            if (State.Instance is null)
            {
                GD.PrintErr("❌ State.Instance est NULL !");
                return;
            }

            if (State.Instance.can_enter_forest is false) // Vérifie si l'accès est autorisé
            {
                GD.Print("❌ Accès interdit à la forêt !");
                player.BlockMovement(1.5f); // Bloque le joueur 1.5 secondes
            }
            else
            {
                GD.Print("✅ Accès autorisé !");
            }
        }
    }
}
