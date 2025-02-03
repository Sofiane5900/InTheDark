using System;
using DialogueManagerRuntime;
using Godot;

public partial class Barriere : Area2D
{
    [Export]
    public Resource DialogueResource; //

    [Export]
    public string DialogueStart = "start";

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered; // Connection au signal body entered de l'Area2D
    }

    private void OnBodyEntered(Node body)
    {
        if (body is Character player)
        {
            // Si l'accès à la forêt est interdit
            if (State.Instance.can_enter_forest is false)
            {
                GD.Print("Accès interdit !");

                // On recule le joueur de 10 pixels sur l'axe Y
                player.MoveAndCollide(new Vector2(0, 10));

                // On lance le dialogue après avoir reculé le joueur
                StartDialogue();
            }
            else
            {
                GD.Print("Accès autorisé !");
            }
        }
    }

    private void StartDialogue()
    {
        if (DialogueResource is null)
        {
            // On copie le fonctionnement du script Actionnable.cs
            var dialogue = DialogueManager.ShowDialogueBalloon(DialogueResource, DialogueStart);
            dialogue.ProcessMode = DialogueManager.ProcessModeEnum.Always;

            GetTree().Paused = true; // Pause
            DialogueManager.DialogueEnded += Unpause; // Unpause après la fin du dialogue (signal DialogueEnded)
        }
        else
        {
            GD.PrintErr("Aucun DialogueResource assigné !");
        }
    }

    private void Unpause(Resource dialogueResource)
    {
        GetTree().Paused = false; // Pause

        if (GetTree().CurrentScene.HasNode("Character"))
        {
            var player = GetTree().CurrentScene.GetNode<Character>("Character");
            player.BlockMovement(1.5f);
        }
    }
}
