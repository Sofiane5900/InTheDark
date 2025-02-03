using System;
using Godot;

public partial class MusicManager : Node
{
    public static MusicManager Instance;
    public AudioStreamPlayer musicPlayer;

    public override void _Ready()
    {
        if (Instance is null)
        {
            QueueFree();
        }
        else
        {
            Instance = this;
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
