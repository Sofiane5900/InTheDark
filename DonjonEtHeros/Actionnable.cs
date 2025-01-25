using System;
using DialogueManagerRuntime;
using Godot;

public partial class Actionnable : Area2D
{
    [Export]
    public Resource DialogueResource;

    [Export]
    public string DialogueStart = "start";

    public void Action()
    {
        DialogueManager.ShowDialogueBalloon(DialogueResource, DialogueStart);
        GD.Print("Actionnable, ça marche.");
    }
}
