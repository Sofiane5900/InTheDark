using System;
using Godot;

public partial class State : Node
{
    // public static bool CanMove;
    public string AppleStatus = "";

    public int CurrentHealth = 100;
    public int MaxHealth = 100;
    public int Damage = 10;

    // has_met variables
    public bool has_met_pouleto = false;

    // death variables
    public bool pouleto_is_dead = false;
}
