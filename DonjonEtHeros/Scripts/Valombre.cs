using Godot;

public partial class Valombre : Node
{
    [Export]
    private AudioStream _valombreMusic;

    public override async void _Ready()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); //  Attendre une frame

        if (MusicManager.Instance is null)
        {
            GD.PrintErr(" MusicManager.Instance est NULL après attente !");
            return;
        }

        if (_valombreMusic is null)
        {
            GD.PrintErr("Aucune musique assignée dans l'éditeur !");
            return;
        }

        MusicManager.Instance.PlayMusic(_valombreMusic);
    }
}
