using Godot;

// ? Script relatif a la scene Foret
public partial class Foret : Node
{
    [Export]
    private AudioStream _foretMusic;

    public override async void _Ready()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); //  Attendre une frame

        if (MusicManager.Instance is null)
        {
            GD.PrintErr(" MusicManager.Instance est NULL après l'attente !");
            return;
        }

        if (_foretMusic is null)
        {
            GD.PrintErr("Aucune musique assignée dans l'inspector !");
            return;
        }

        MusicManager.Instance.PlayMusic(_foretMusic);
    }
}
