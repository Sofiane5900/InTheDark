using System.Threading.Tasks;
using Godot;

public partial class EndCredits : Control
{
    [Export]
    public float DisplayTime = 15f; // Temps avant fermeture (en secondes)

    public override async void _Ready()
    {
        GD.Print("🎬 Affichage des crédits...");

        // ✅ Attendre la fin des crédits (ex: 5 secondes)
        await Task.Delay((int)(DisplayTime * 1000));

        GD.Print("🚪 Fin du jeu, fermeture...");
        GetTree().Quit();
    }
}
