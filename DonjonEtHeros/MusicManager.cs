using System;
using Godot;

public partial class MusicManager : Node
{
    private static MusicManager Instance;
    private AudioStreamPlayer musicPlayer;

    private override void _Ready()
    {
        if (Instance is null)
    }
}
