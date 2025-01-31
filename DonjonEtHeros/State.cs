using System;
using Godot;

public partial class State : Node
{
	// public static bool CanMove;
	public string AppleStatus = "";

	public int CurrentHealth = 10;
	public int MaxHealth = 100;
	public int Damage = 10;

	// has_met variables
	public bool has_met_pouleto = false;
}
