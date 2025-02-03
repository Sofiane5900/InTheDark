using System;
using DialogueManagerRuntime;
using Godot;

public partial class Actionnable : Area2D
{
    [Export]
    public Resource DialogueResource;

    [Export]
    public string DialogueStart = "start";

    private bool isDialogueActive = true;

    public void Action()
    {
        if (isDialogueActive is true)
        {
            // un dialogue ne peux pas se lancer deux fois en meme temps
            return;
        }

        isDialogueActive = true; // Dialogue en cours

        var dialogue = DialogueManager.ShowDialogueBalloon(DialogueResource, DialogueStart);
        dialogue.ProcessMode = DialogueManager.ProcessModeEnum.Always; // process always = ne sera jamais en pause
        GetTree().Paused = true; // Pause du jeu
        DialogueManager.DialogueEnded += Unpause; // Connexion au signal UnPause
        GD.Print("Actionnable activé.");
    }

    private void Unpause(Resource dialogueResource)
    {
        // On verifie si l'objet Actionnable a était supprimé ou retiré de la scene
        // ? IsInstanceValid = Verification de si l'objet est toujours en mémoire
        // ? IsInsideTree = Vérification de si l'objet est toujours dans l'arbre
        if (!IsInstanceValid(this) || !IsInsideTree())
        {
            return;
        }

        GetTree().Paused = false;
        isDialogueActive = false; // On rénitialise le bool a sa valeur de défaut
        GD.Print("Reprise du jeu après dialogue.");
    }

    public override void _ExitTree()
    {
        DialogueManager.DialogueEnded -= Unpause; // Déconnexion du signal quand l'objet est supprimé
    }
}
