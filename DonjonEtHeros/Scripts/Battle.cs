using Godot;

public partial class Battle : Control
{
    private Panel Textbox;

    private Panel ActionsPanel;

    private Label TextboxLabel;

    // Je crée un signal textbox_cloesd
    [Signal]
    public delegate void textbox_closedEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Textbox = GetNode<Panel>("Textbox");
        ActionsPanel = GetNode<Panel>("ActionsPanel");
        TextboxLabel = GetNode<Label>("Textbox/Label");
        Textbox.Visible = false;
        ActionsPanel.Visible = false;
        DisplayText("Ceci est un test");

        // On connecte notre signal à notre méthode
        Connect("textbox_closed", Callable.From(OnSignalEmitted));
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionPressed("ui_accept") && Textbox.Visible)
        {
            Textbox.Visible = false;
            EmitSignal("textbox_closed");
        }
    }

    private void OnSignalEmitted()
    {
        ActionsPanel.Visible = true;
    }

    private string DisplayText(string text)
    {
        Textbox.Visible = true;
        TextboxLabel.Text = text;
        return text;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
