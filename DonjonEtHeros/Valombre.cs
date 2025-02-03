using Godot;

public partial class ValombreScene : Node
{
    [Export]
    private AudioStream _valombreMusic;

    public override void _Ready()
    {
        MusicManager.Instance?.PlayMusic(_valombreMusic);
    }
}
