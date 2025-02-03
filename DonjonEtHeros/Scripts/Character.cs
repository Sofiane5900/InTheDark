using System;
using Godot;

public partial class Character : CharacterBody2D
{
    [Export]
    private int _speed = 50;
    private Vector2 _currentVelocity;
    private AnimationPlayer _animationPlayer;

    private Area2D _actionnableFinder;

    public override void _Ready()
    {
        base._Ready();
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _actionnableFinder = GetNode<Area2D>("Direction/ActionnableFinder");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        HandleInput();
        Velocity = _currentVelocity;
        // MoveAndSlide est une methode de Godot qui permet de déplacer un CharacterBody2D en utilisant sa currentVelocity
        MoveAndSlide();
    }

    private void HandleInput()
    {
        _currentVelocity = Vector2.Zero;
        // L'Axe Y est inversé, il commence en haut à gauche et Y augmente en descendant
        if (Input.IsActionPressed("ui_up"))
        {
            _currentVelocity.Y -= 1;
        }
        else if (Input.IsActionPressed("ui_down"))
        {
            _currentVelocity.Y += 1;
        }
        else if (Input.IsActionPressed("ui_left"))
        {
            _currentVelocity.X -= 1;
        }
        else if (Input.IsActionPressed("ui_right"))
        {
            _currentVelocity.X += 1;
        }
        _currentVelocity *= _speed;

        PlayAnimations();
    }

    // UnhandledInput est une methode de Godot qui permet de gérer les inputs qui n'ont pas été traités par d'autres méthodes
    public override async void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        // TODO : Le personnage doit devenir "idle" quand il parle à un NPC
        if (Input.IsActionJustPressed("ui_accept"))
        {
            Godot.Collections.Array<Area2D> actionnables = _actionnableFinder.GetOverlappingAreas();
            if (actionnables.Count > 0)
            {
                GD.Print("Actionnable trouvé : " + actionnables.Count);
                (actionnables[0] as Actionnable).Action();
                _currentVelocity = Vector2.Zero;
            }
        }
    }

    private void PlayAnimations()
    {
        // Nos animations se lancent en fonction de la direction du Character
        if (_currentVelocity == Vector2.Zero)
        {
            _animationPlayer.Play("idle");
        }
        if (_currentVelocity.Y < 0)
        {
            _animationPlayer.Play("walk_up");
        }
        if (_currentVelocity.Y > 0)
        {
            _animationPlayer.Play("walk_down");
        }
        if (_currentVelocity.X < 0)
        {
            _animationPlayer.Play("walk_left");
        }
        if (_currentVelocity.X > 0)
        {
            _animationPlayer.Play("walk_right");
        }
    }

    public async void BlockMovement(float duration = 1.0f)
    {
        GD.Print("🚧 Mouvement bloqué !");
        _currentVelocity = Vector2.Zero; // Stoppe immédiatement
        SetPhysicsProcess(false); // Désactive `_PhysicsProcess()`

        await ToSignal(GetTree().CreateTimer(duration), "timeout");

        GD.Print("🔓 Mouvement débloqué !");
        SetPhysicsProcess(true); // Réactive `_PhysicsProcess()`
    }
}
