using System;
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

    private State stateScript;
    private BaseEnemy enemyScript;
    private Texture enemyTexture;

    [Export]
    private Button RunButton;

    [Export]
    private Resource Enemy;

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
        stateScript = (State)GetNode("/root/State");
        enemyScript = new BaseEnemy();
        // Player & Ennemy Health Bar
        PlayerHealthBar = GetNode<ProgressBar>("PlayerPanel/PlayerData/ProgressBar");
        EnemyHealthBar = GetNode<ProgressBar>("EnemyContainer/ProgressBar");

        // Buttons
        RunButton = GetNode<Button>("ActionsPanel/Actions/Run");
        RunButton.Pressed += HandleButtonPressed;
        Textbox.Visible = false;
        ActionsPanel.Visible = false;
        DisplayText("Un pouleto revanchard apparaît !");

        // On connecte notre signal à nos méthodes
        Connect("textbox_closed", Callable.From(CloseActionsPanel));
        SetHealth(PlayerHealthBar, stateScript.CurrentHealth, stateScript.MaxHealth);
        SetHealth(EnemyHealthBar, enemyScript.health, enemyScript.health);
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

    private async void HandleButtonPressed()
    {
        DisplayText("Vous avez fui le combat !");
        await ToSignal(GetTree().CreateTimer(2), "timeout");
        GetTree().Quit();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
