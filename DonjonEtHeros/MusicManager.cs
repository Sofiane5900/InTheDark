using Godot;

public partial class MusicManager : Node
{
    public static MusicManager Instance { get; private set; }
    private AudioStreamPlayer _musicPlayer;

    public override void _Ready()
    {
        if (Instance is null)
        {
            Instance = this;
            _musicPlayer = new AudioStreamPlayer();
            AddChild(_musicPlayer);
        }
        else
        {
            QueueFree();
        }
    }

    public void PlayMusic(AudioStream music)
    {
        if (music is null)
        {
            GD.PrintErr("Aucune musique à jouer");
            return;
        }

        if (_musicPlayer.Stream == music)
        {
            GD.Print("La musique est déjà en cours");
            return;
        }

        _musicPlayer.Stream = music;
        if (music is AudioStreamOggVorbis ogg)
        {
            ogg.Loop = true;
        }
        _musicPlayer.Play();
    }
}
