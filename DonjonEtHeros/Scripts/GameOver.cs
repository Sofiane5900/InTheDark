using System.Threading.Tasks;
using Godot;

public partial class GameOver : Control
{
    [Export]
    public float DisplayTime = 5f; // Temps avant fermeture des creditrs float = secondes

    public override async void _Ready()
    {
        GD.Print("Affichage des crédits...");

        //  Attendre la fin des crédits
        await Task.Delay((int)(DisplayTime * 1000)); // on multiplie par 1000 pour convertir en millisecondes (task.Delay attend des millisecondes)

        GD.Print(" Fin du jeu, fermeture...");
        GetTree().Quit();
    }
}
