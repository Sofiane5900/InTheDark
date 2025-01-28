using System;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Threading.Tasks;
using Godot;

public partial class Battle : Control
{
    private Panel Textbox;

    private Panel ActionsPanel;

    private Label TextboxLabel;

    private ProgressBar PlayerHealthBar;

    private ProgressBar EnemyHealthBar;

    private int currentPlayerHealth = 0;
    private int currentEnemyHealth = 0;

    private State stateScript;

    // private BaseEnemy enemyScript;
    private Texture EnemyTexture;

    [Export]
    private Button RunButton;

    [Export]
    private Button AttackButton;

    [Export]
    private BaseEnemy Enemy;

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
        EnemyTexture = GetNode<TextureRect>("EnemyContainer/Enemy").Texture;
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        // Buttons
        RunButton = GetNode<Button>("ActionsPanel/Actions/Run");
        AttackButton = GetNode<Button>("ActionsPanel/Actions/Attack");
        RunButton.Pressed += HandleRunButton;
        AttackButton.Pressed += HandleAttackButton;

        Textbox.Visible = false;
        ActionsPanel.Visible = false;
        DisplayText($"Un {Enemy.name} apparaît devant vous !");

        // On connecte notre signal à nos méthodes
        Connect("textbox_closed", Callable.From(CloseActionsPanel));
        SetHealth(PlayerHealthBar, stateScript.CurrentHealth, stateScript.MaxHealth);
        SetHealth(EnemyHealthBar, Enemy.health, Enemy.health);
        currentPlayerHealth = stateScript.CurrentHealth;
        currentEnemyHealth = Enemy.health;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionPressed("ui_accept") && Textbox.Visible)
        {
            Textbox.Visible = false;
            // Lorsque que j'appuie sur la touche "ui_accept" j'emet mon signal
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
        Textbox.Visible = true;
        TextboxLabel.Text = text;
        return text;
    }

    private async void HandleRunButton()
    {
        DisplayText("Vous avez fuit le combat !");
        ActionsPanel.Visible = false;
        GetTree().Paused = true;
        await ToSignal(GetTree().CreateTimer(2), "timeout");
        GetTree().Quit();
    }

    private async void HandleAttackButton()
    {
        DisplayText("Vous avez infligé " + stateScript.Damage + " points de dégâts à l'ennemi !");
        ActionsPanel.Visible = false;
        currentEnemyHealth = Math.Max(0, currentEnemyHealth - stateScript.Damage); // On evite que les PV deviennent négatif
        SetHealth(EnemyHealthBar, currentEnemyHealth, Enemy.health);
        AnimationPlayer.Play("enemy_damaged");
        await ToSignal(GetTree().CreateTimer(1.2), "timeout");
    }

    private async void EnemyAttack()
    {
        DisplayText("L'ennemi vous attaque !");
        ActionsPanel.Visible = false;
        currentPlayerHealth = Math.Max(0, currentPlayerHealth - Enemy.damage); // On evite que les PV deviennent négatif
        SetHealth(PlayerHealthBar, currentPlayerHealth, stateScript.MaxHealth);
        // TODO : On pourrait renamer cette animation en "player_damaged"
        AnimationPlayer.Play("shake");
        await ToSignal(GetTree().CreateTimer(1.2), "timeout");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
