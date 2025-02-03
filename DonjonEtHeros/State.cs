using System;
using Godot;

// ? Je stocke ici les variables globales du jeu, les access, statut, les stats du joueur, etc..
public partial class State : Node
{
    public static State Instance { get; private set; }

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
    }

    public string AppleStatus = "";
    public string CryptStatus = "";

    public int CurrentHealth = 100;
    public int MaxHealth = 100;
    public int Damage = 10;

    // has_met variables
    public bool has_met_alphonse = false;

    // has_seen variables
    public bool has_seen_symbols_forest = false;

    // can_enter variables
    public bool can_enter_forest = false;
}
