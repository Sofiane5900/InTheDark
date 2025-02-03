using Godot;

// ? Script relatif a la scene Valombre
public partial class Valombre : Node
{
    [Export]
    private AudioStream _valombreMusic;

    public override async void _Ready()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); //  Attendre une frame

        if (MusicManager.Instance is null)
        {
            GD.PrintErr(" MusicManager.Instance est NULL après l'attente !");
            return;
        }

        if (_valombreMusic is null)
        {
            GD.PrintErr("Aucune musique assignée dans l'inspector !");
            return;
        }

        MusicManager.Instance.PlayMusic(_valombreMusic);
    }
}
