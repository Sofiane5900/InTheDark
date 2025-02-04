using System;
using Godot;

// ! Le joueur ne doit pas pouvoir spam entre les actions
// TODO : Ajouter du delay "ui_accept" pour éviter le spam
public partial class Battle : Control
{
    private Panel Textbox;

    private Panel ActionsPanel;

    private Label TextboxLabel;

    private ProgressBar PlayerHealthBar;

    private ProgressBar EnemyHealthBar;

    private int currentPlayerHealth = 0;
    private int currentEnemyHealth = 0;
    private bool isDefending = false;

    private State stateScript;

    // private BaseEnemy enemyScript;
    private TextureRect EnemyTexture;

    [Export]
    private Button AttackButton;

    [Export]
    private Button DefendButton;

    [Export]
    private Button RunButton;

    [Export]
    public BaseEnemy EnemyResource;

    private AnimationPlayer AnimationPlayer;

    // Je crée un signal textbox_closed
    [Signal]
    public delegate void textbox_closedEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Text & Actions Panel
        Textbox = GetNode<Panel>("Textbox");
        ActionsPanel = GetNode<Panel>("ActionsPanel");
        TextboxLabel = GetNode<Label>("Textbox/Label");
        // Scripts
        stateScript = (State)GetNode("/root/State");
        // Player & Ennemy Nodes
        PlayerHealthBar = GetNode<ProgressBar>("PlayerPanel/PlayerData/ProgressBar");
        EnemyHealthBar = GetNode<ProgressBar>("EnemyContainer/ProgressBar");
        EnemyTexture = GetNode<TextureRect>("EnemyContainer/EnemyTexture");
        EnemyTexture.Texture = EnemyResource.texture as Texture2D;
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        // Buttons
        RunButton = GetNode<Button>("ActionsPanel/Actions/Run");
        AttackButton = GetNode<Button>("ActionsPanel/Actions/Attack");
        DefendButton = GetNode<Button>("ActionsPanel/Actions/Defend");
        AttackButton.Pressed += HandleAttackButton;
        RunButton.Pressed += HandleRunButton;
        if (EnemyResource.name == "Seigneur Liche")
        {
            RunButton.Disabled = true;
        }
        GD.Print($"👿 Nom de l'ennemi actuel : {EnemyResource.name}");
        DefendButton.Pressed += HandleDefendButton;

        Textbox.Visible = false;
        ActionsPanel.Visible = false;
        DisplayText($"Un {EnemyResource.name} apparaît devant vous !");

        // On connecte notre signal à nos méthodes
        Connect("textbox_closed", Callable.From(CloseActionsPanel));
        SetHealth(PlayerHealthBar, stateScript.CurrentHealth, stateScript.MaxHealth);
        SetHealth(EnemyHealthBar, EnemyResource.health, EnemyResource.health);
        currentPlayerHealth = stateScript.CurrentHealth;
        currentEnemyHealth = EnemyResource.health;
    }

    // TODO : Empecher de spammer la touche "ui_accept" pour fermer le textbox et spam des actions
    public override async void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionPressed("ui_accept") && Textbox.Visible)
        {
            Textbox.Visible = false;
            // Lorsque que j'appuie sur la touche "ui_accept" j'emet mon signal
            await ToSignal(GetTree().CreateTimer(0.3), "timeout");
            EmitSignal("textbox_closed");
        }
    }

    private void SetHealth(ProgressBar progressBar, int currentHealth, int maxHealth)
    {
        progressBar.MaxValue = maxHealth;
        progressBar.Value = currentHealth;
        progressBar.GetNode<Label>("Label").Text = $"PV: {currentHealth}/{maxHealth}";
    }

    private void CloseActionsPanel()
    {
        ActionsPanel.Visible = true;
    }

    private string DisplayText(string text)
    {
        ActionsPanel.Visible = false;
        Textbox.Visible = true;
        TextboxLabel.Text = text;
        return text;
    }

    private async void HandleRunButton()
    {
        DisplayText("Vous avez fuit le combat !");
        GetTree().Paused = true;
        await ToSignal(GetTree().CreateTimer(2), "timeout");
        GetTree().Paused = false;
        BattleManager.Instance.EndBattle();
    }

    private async void HandleAttackButton()
    {
        DisplayText("Vous avez infligé " + stateScript.Damage + " points de dégâts à l'ennemi !");
        currentEnemyHealth = Math.Max(0, currentEnemyHealth - stateScript.Damage); // On evite que les PV deviennent négatif
        SetHealth(EnemyHealthBar, currentEnemyHealth, EnemyResource.health);
        AnimationPlayer.Play("enemy_damaged");
        await ToSignal(AnimationPlayer, "animation_finished");
        if (currentEnemyHealth == 0)
        {
            EnemyDeath();
        }
        else
        {
            EnemyAttack();
        }
    }

    private async void HandleDefendButton()
    {
        isDefending = true;
        DisplayText("Vous avez defendu l'ataque de l'ennemi !");
        await ToSignal(GetTree().CreateTimer(1.2), "timeout");
        ActionsPanel.Visible = false;
        EnemyAttack();
    }

    private async void EnemyAttack()
    {
        int actualDamage = EnemyResource.damage; // On garde la valeur originale

        if (isDefending)
        {
            isDefending = false;
            DisplayText("Vous avez réduit l'attaque de l'ennemi de moitié !");
            actualDamage /= 2; // Réduction temporaire des dégâts

            await ToSignal(GetTree().CreateTimer(1.2), "timeout");
            AnimationPlayer.Play("mini_shake");
        }
        else
        {
            DisplayText("L'ennemi vous attaque !");
            AnimationPlayer.Play("shake");
            await ToSignal(AnimationPlayer, "animation_finished");
        }

        // On applique les dégats sans modifier la valeur originale
        currentPlayerHealth = Math.Max(0, currentPlayerHealth - actualDamage);
        SetHealth(PlayerHealthBar, currentPlayerHealth, stateScript.MaxHealth);

        GD.Print("Vérification des PV de l'ennemi...");
        if (EnemyResource.health == 0)
        {
            GD.Print("L'ennemi est mort ! Je lance EnemyDeath()");
            EnemyDeath();
        }

        GD.Print("Vérification des PV du joueur...");
        if (currentPlayerHealth == 0)
        {
            GD.Print("Le joueur est mort ! Je lance PlayerDeath()");
            PlayerDeath();
        }
    }

    public async void EnemyDeath()
    {
        GD.PrintErr("Ennemy DEAD!");
        AnimationPlayer.Play("enemy_death");
        await ToSignal(AnimationPlayer, "animation_finished");
        DisplayText($"Vous avez vaincu le {EnemyResource.name} !");
        await ToSignal(GetTree().CreateTimer(2), "timeout");
        if (EnemyResource.name == "Seigneur Liche")
        {
            GD.Print("🎬 Lancement des crédits...");
            GetTree().ChangeSceneToFile("res://Scenes/EndCredits.tscn"); // Lance la scène de fin
        }
        else
        {
            GD.Print("Calling EndBattle...");
            BattleManager.Instance.EndBattle();
        }
    }

    public async void PlayerDeath()
    {
        GD.PrintErr("Player DEAD!");
        await ToSignal(AnimationPlayer, "animation_finished");
        DisplayText("Vous avez été vaincu !");
        await ToSignal(GetTree().CreateTimer(2), "timeout");
        GetTree().ChangeSceneToFile("res://Scenes/GameOver.tscn");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
