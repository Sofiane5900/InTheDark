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
        if (isDefending is true)
        {
            isDefending = false;
            DisplayText("Vous avez reduit l'attaque de l'ennemi de moitié !");
            EnemyResource.damage = EnemyResource.damage / 2;
            currentPlayerHealth = Math.Max(0, currentPlayerHealth - EnemyResource.damage); // On evite que les PV deviennent négatif
            SetHealth(PlayerHealthBar, currentPlayerHealth, stateScript.MaxHealth);
            await ToSignal(GetTree().CreateTimer(1.2), "timeout");
            AnimationPlayer.Play("mini_shake");
        }
        else
        {
            DisplayText("L'ennemi vous attaque !");
            currentPlayerHealth = Math.Max(0, currentPlayerHealth - EnemyResource.damage); // On evite que les PV deviennent négatif
            SetHealth(PlayerHealthBar, currentPlayerHealth, stateScript.MaxHealth);
            // TODO : On pourrait rename cette animation en "player_damaged"
            AnimationPlayer.Play("shake");
            await ToSignal(AnimationPlayer, "animation_finished");
        }
        GD.Print("Checking if enemy health is zero...");
        if (EnemyResource.health == 0)
        {
            GD.Print("Enemy health is zero, calling EnemyDeath!");
            EnemyDeath();
        }
    }

    public async void EnemyDeath()
    {
        GD.PrintErr("Ennemy DEAD!");
        AnimationPlayer.Play("enemy_death");
        await ToSignal(AnimationPlayer, "animation_finished");
        DisplayText($"Vous avez vaincu le {EnemyResource.name} !");
        await ToSignal(GetTree().CreateTimer(2), "timeout");
        GD.Print("Calling EndBattle...");
        BattleManager.Instance.EndBattle();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
