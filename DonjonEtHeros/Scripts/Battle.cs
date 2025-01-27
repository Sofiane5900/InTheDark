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

    private State state;

    [Export]
    private Button RunButton;

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
        state = (State)GetNode("/root/State");
        // Player
        PlayerHealthBar = GetNode<ProgressBar>("PlayerPanel/PlayerData/ProgressBar");
        // Buttons
        RunButton = GetNode<Button>("ActionsPanel/Actions/Run");
        RunButton.Pressed += HandleButtonPressed;
        Textbox.Visible = false;
        ActionsPanel.Visible = false;
        DisplayText("Un pouleto revanchard apparaît !");

        // On connecte notre signal à nos méthodes
        Connect("textbox_closed", Callable.From(CloseActionsPanel));
        SetHealth(PlayerHealthBar, state.CurrentHealth, state.MaxHealth);
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
