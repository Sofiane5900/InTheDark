using Godot;

public partial class Battle : Control
{
    private Panel Textbox;

    private Panel ActionsPanel;

    private Label TextboxLabel;

    [Export]
    private Button RunButton;

    // Je crée un signal textbox_closed
    [Signal]
    public delegate void textbox_closedEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Textbox = GetNode<Panel>("Textbox");
        ActionsPanel = GetNode<Panel>("ActionsPanel");
        TextboxLabel = GetNode<Label>("Textbox/Label");
        RunButton = GetNode<Button>("ActionsPanel/Actions/Run");
        RunButton.Pressed += HandleButtonPressed;
        Textbox.Visible = false;
        ActionsPanel.Visible = false;
        DisplayText("Ceci est un test");

        // On connecte notre signal à nos méthodes
        Connect("textbox_closed", Callable.From(CloseActionsPanel));
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

    private void HandleButtonPressed()
    {
        Connect("textbox_closed", Callable.From(HandleButtonPressed));
        DisplayText("Vous avez fui le combat.");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
