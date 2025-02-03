using System.Threading.Tasks;
using DialogueManagerRuntime;
using Godot;

public partial class BossArea : Area2D
{
    private CharacterBody2D player;
    private Node2D boss;
    private bool isDialogueActive = false;

    [Export]
    public Resource BossDialogueResource; // dialogue du boss

    [Export]
    public string BattleSceneName = "BattleMap"; // map classique pour le combat

    [Export]
    public string BossEnemyResource = "Liche"; // fichier .tres du boss

    public override void _Ready()
    {
        BodyEntered += OnPlayerEntered;
        GD.Print(" BossArea initialisée !");
    }

    private void OnPlayerEntered(Node body)
    {
        if (body is CharacterBody2D character)
        {
            GD.Print($"Détection du joueur : {body.Name}");

            player = character;
            boss = GetNodeOrNull<Node2D>("../Liche");

            if (boss is null)
            {
                GD.PrintErr("Liche introuvable !");
                return;
            }

            GD.Print($" Boss trouvé : {boss.Name}");

            player.SetPhysicsProcess(false); // Joueur en pause
            GD.Print("Le joueur ne peut plus bouger.");

            // Je lance la cutscene
            StartCutscene();
        }
    }

    private async void StartCutscene()
    {
        GD.Print("Début de la cutscene...");

        // On déplace le joueur vers le boss
        Vector2 targetPosition = boss.Position + new Vector2(0, 50);
        await MovePlayerToBoss(targetPosition);

        //On lance le dialogue du boss
        StartBossDialogue();
    }

    private async Task MovePlayerToBoss(Vector2 target)
    {
        float speed = 20f; // ✅ Une vitesse plus raisonnable maintenant que delta n'interfère pas
        float threshold = 0.05f; // ✅ Réduction du seuil pour un arrêt plus précis

        GD.Print(" Déplacement du joueur vers le boss...");

        while (player.Position.DistanceTo(target) > threshold)
        {
            player.Position = player.Position.MoveToward(target, speed / 60f); // ✅ Ajustement du mouvement pour 60 FPS

            await Task.Delay(16); // ✅ On attend une frame (16ms pour 60 FPS)
        }

        GD.Print("🦸 Le joueur est en position !");
    }

    private void StartBossDialogue()
    {
        if (isDialogueActive is true)
        {
            GD.Print(" Dialogue déjà en cours...");
            return;
        }

        if (BossDialogueResource is null)
        {
            GD.PrintErr(" Aucun dialogue assigné au boss !");
            return;
        }

        isDialogueActive = true; // On empeche les doublons de dialogues
        GD.Print("Lancement du dialogue du boss...");

        var dialogue = DialogueManager.ShowDialogueBalloon(BossDialogueResource, "boss_liche");
        dialogue.ProcessMode = DialogueManager.ProcessModeEnum.Always;
        GetTree().Paused = true;

        DialogueManager.DialogueEnded += Unpause; // Connexion au signal
    }

    // ? Si je ne met pas de paramètre, le compilateur va me dire que la méthode ne correspond pas au delegate
    private void Unpause(Resource dialogueResource)
    {
        if (!IsInstanceValid(this) || !IsInsideTree())
        {
            GD.PrintErr("⚠ BossArea supprimée, impossible d'unpause !");
            return;
        }

        GetTree().Paused = false;
        isDialogueActive = false; //  On renitialise le bool
        GD.Print("🔄 Reprise du jeu après dialogue.");

        //  On démarre le combat après le dialogue
        StartBattle();
    }

    private async void StartBattle()
    {
        GD.Print(" Début du combat contre le Seigneur Liche !");
        await BattleManager.StartBattle(BattleSceneName, BossEnemyResource);
    }

    public override void _ExitTree()
    {
        DialogueManager.DialogueEnded -= Unpause; // ✅ Déconnexion propre
        GD.Print(" BossArea supprimée");
    }
}
