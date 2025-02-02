using System;
using Godot;

public partial class BaseEnemy : Resource
{
    [Export]
    public string name = "Enemy";

    [Export]
    public Texture texture = null;

    [Export]
    public int health = 100;

    [Export]
    public int damage = 10;
}
