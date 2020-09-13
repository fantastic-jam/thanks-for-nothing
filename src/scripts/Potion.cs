using System;
using Godot;

public class Potion : StaticBody2D
{
    private const float Speed = 300.0f;
    private readonly RandomNumberGenerator _rand = new RandomNumberGenerator();

    [Export] private PackedScene _labelPrefab = null;

    public Vector2 Destination;

    private Area2D _area;
    private Vector2 _origin;
    private float _totalDistance;
    private Sprite _sprite;

    public override void _Ready()
    {
        _rand.Randomize();
        _area = GetNode<Area2D>("Area2D");
        _sprite = GetNode<Sprite>("Sprite");
        _origin = Position;
        _totalDistance = (_origin - Destination).Length();
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Destination == Vector2.Zero) return;
        if ((Position - Destination).Length() < 5.0f)
        {
            CallDeferred(nameof(OnDestinationReached));
            return;
        }

        TravelToDestination(delta);
    }

    private void TravelToDestination(float delta)
    {
        float progression = GetTravelProgression();
        float curve = GetTravelCurve(progression);
        _sprite.Scale = (1 + curve) * Vector2.One;
        _sprite.Rotation += 10.0f * delta;
        Translate(Position.DirectionTo(Destination) * Speed * delta);
    }

    private float GetTravelProgression()
    {
        float remainingDistance = (Position - Destination).Length();
        return (_totalDistance - remainingDistance) / _totalDistance;
    }

    private float GetTravelCurve(float progression)
    {
        return (0.5f - Math.Abs(progression - 0.5f)) * 2.0f;
    }

    public void OnEnter(Node other)
    {
        if (!(other is Player player))
            return;
        ApplyTo(player);
        QueueFree();
    }

    private enum Effect
    {
        Heal,
        Poison,
        SpeedUp,
        SpeedDown
    }

    private static readonly Array EffectList = Enum.GetValues(typeof(Effect));

    private void ApplyTo(Player player)
    {
        var effect = (Effect) _rand.RandiRange(0, EffectList.Length - 1);
        var floatingLabel = (Node2D)_labelPrefab.Instance();
        var label = floatingLabel.GetNode<Label>("Label");
        switch (effect)
        {
            case Effect.Heal:
                player.Health = Math.Min(player.Health + 20, player.MaxHealth);
                label.Text = "health++";
                label.AddColorOverride("font_color", Color.Color8(0, 255, 0));
                break;
            case Effect.Poison:
                player.Health = Math.Max(player.Health - 20, 1);
                label.Text = "health--";
                label.AddColorOverride("font_color", Color.Color8(255, 0, 0));
                break;
            case Effect.SpeedUp:
                player.Speed += 20;
                label.Text = "speed++";
                label.AddColorOverride("font_color", Color.Color8(0, 255, 0));
                break;
            case Effect.SpeedDown:
                player.Speed = Math.Max(player.Speed - 10, 100);
                label.Text = "speed--";
                label.AddColorOverride("font_color", Color.Color8(255, 0, 0));
                break;
        }
        player.AddChild(floatingLabel);
    }

    private void OnDestinationReached()
    {
        Position = Destination;
        ZIndex = 0;
        _sprite.Scale = Vector2.One;
        Destination = Vector2.Zero;
        _area.Monitoring = true;
    }
}
