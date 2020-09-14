using Godot;

public class FloatingText : Node2D
{
    public string Text { get; set; }
    public Color Color { get; set; }

    private Label _label;
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _label = GetNode<Position2D>("Position2D").GetNode<Label>("Label");
        _label.Text = Text;
        _label.AddColorOverride("font_color", Color);
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }
}
