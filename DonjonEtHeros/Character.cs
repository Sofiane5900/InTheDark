using System;
using Godot;

public partial class Character : CharacterBody2D
{
    [Export]
    private int speed = 50;
    private Vector2 currentVelocity;
    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        base._Ready();
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        HandleInput();
        Velocity = currentVelocity;
        // MoveAndSlide est une methode de Godot qui permet de depalcer un CharacterBody2D en utilisant sa currentVelocity
        MoveAndSlide();
    }

    private void HandleInput()
    {
        currentVelocity = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        currentVelocity *= speed;
        PlayAnimations();
        Console.WriteLine(Vector2.Axis.X);
        Console.WriteLine(Vector2.Axis.Y);
    }

    private void PlayAnimations()
    {
        if (currentVelocity == Vector2.Zero)
        {
            animationPlayer.Play("idle");
        }
        if (currentVelocity.Y < 0)
        {
            animationPlayer.Play("walk_up");
        }
        if (currentVelocity.Y > 0)
        {
            animationPlayer.Play("walk_down");
        }
        if (currentVelocity.X < 0)
        {
            animationPlayer.Play("walk_left");
        }
        if (currentVelocity.X > 0)
        {
            animationPlayer.Play("walk_right");
        }
    }
}
