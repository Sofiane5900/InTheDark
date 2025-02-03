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
        // On a défini une variable dialogue qui contient une méthode de DialogueManager
        var dialogue = DialogueManager.ShowDialogueBalloon(DialogueResource, DialogueStart);
        // On a défini le PROCESS MODE de notre dialogue en "always", meme si notre Tree est paused, le dialogue continue.
        dialogue.ProcessMode = DialogueManager.ProcessModeEnum.Always;

        GetTree().Paused = true; // Pause du jeu
        // Je connecte ma Méthode Unpause a l'événement DialogueEnded a DialogueEnded
        DialogueManager.DialogueEnded += Unpause;
        GD.Print("Actionnable fonctionnelle.");
    }

    private void Unpause(Resource dialogueResource)
    {
        GetTree().Paused = false;
    }
}
