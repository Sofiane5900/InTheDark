using System;
using Godot;

// ? Pattern Singleton pour gérer la musique car on ne veut pas que la musique se coupe brutalement
public partial class MusicManager : Node
{
    public static MusicManager Instance { get; private set; }
    public AudioStreamPlayer musicPlayer;

    public override void _Ready()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
        }
        musicPlayer = new AudioStreamPlayer();
        AddChild(musicPlayer);
        musicPlayer.Autoplay = true;
    }

    public void PlayMusic(AudioStream music)
    {
        // Si la musique est déjà en cours de lecture, on ne la relance pas
        if (musicPlayer.Stream == music)
        {
            return;
        }
        // Le stream doit jouer notre musique
        musicPlayer.Stream = music;
        musicPlayer.Play();
    }
}
