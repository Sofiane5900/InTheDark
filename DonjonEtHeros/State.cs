using System;
using Godot;

public partial class State : Node
{
    public string AppleStatus = "";
    public string CryptStatus = "";

    public int CurrentHealth = 100;
    public int MaxHealth = 100;
    public int Damage = 10;

    // has_met variables
    public bool has_met_alphonse = false;

    // has_seen variables
    public bool has_seen_symbols_forest = false;
}
