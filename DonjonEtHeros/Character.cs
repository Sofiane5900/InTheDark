using System;
using DialogueManagerRuntime;
using Godot;

public partial class Character : CharacterBody2D
{
    [Export]
    private int speed = 50;
    private Vector2 currentVelocity;
    private AnimationPlayer animationPlayer;

    private Area2D actionnableFinder;

    public override void _Ready()
    {
        base._Ready();
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        actionnableFinder = GetNode<Area2D>("Direction/ActionnableFinder");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        HandleInput();
        Velocity = currentVelocity;
        // MoveAndSlide est une methode de Godot qui permet de depalcer un CharacterBody2D en utilisant sa currentVelocity
        MoveAndSlide();
    }

    public interface IDialogueProvider
    {
        void StartDialogue();
    }

    private void HandleInput()
    {
        currentVelocity = Vector2.Zero;
        if (Input.IsActionPressed("ui_up"))
        {
            currentVelocity.Y -= 1;
        }
        else if (Input.IsActionPressed("ui_down"))
        {
            currentVelocity.Y += 1;
        }
        else if (Input.IsActionPressed("ui_left"))
        {
            currentVelocity.X -= 1;
        }
        else if (Input.IsActionPressed("ui_right"))
        {
            currentVelocity.X += 1;
        }
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
