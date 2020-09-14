using System;
using Godot;

public class Potion : Node
{
    private const float Speed = 300.0f;

    [Export] private PackedScene _floatingTextPrefab;

    public Vector2 InitialPosition;
    public float InitialRotation;
    public Vector2 Destination;

    [Export] private float _healthBonusRatio;
    [Export] private float _maxHealthBonusRatio;
    [Export] private float _damageBonusRatio;
    [Export] private float _speedBonusRatio;
    [Export] private string _effectText;
    [Export] private Color _effectTextColor;

    private Node2D _potionObject;
    private Sprite _sprite;
    private Area2D _area;
    private float _totalDistance;

    public override void _Ready()
    {
        _potionObject = GetNode<Node2D>("PotionObject");
        _area = _potionObject.GetNode<Area2D>("Area2D");
        _sprite = _potionObject.GetNode<Sprite>("Sprite");

        _potionObject.Rotation = InitialRotation;
        _potionObject.Position = InitialPosition;
        _totalDistance = (InitialPosition - Destination).Length();
        _area.Connect("body_entered", this, nameof(OnEnter));
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Destination == Vector2.Zero) return;
        if ((_potionObject.Position - Destination).Length() < 5.0f)
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
        _potionObject.Translate(_potionObject.Position.DirectionTo(Destination) * Speed * delta);
    }

    private float GetTravelProgression()
    {
        float remainingDistance = (_potionObject.Position - Destination).Length();
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

    private void ApplyTo(Unit unit)
    {
        ShowEffect(unit);
        unit.MaxHealth = Math.Max(1, unit.MaxHealth + (int) (unit.MaxHealth * _maxHealthBonusRatio));
        unit.Health = Math.Max(1, Math.Min(unit.Health + (int) (unit.MaxHealth * _healthBonusRatio), unit.MaxHealth));
        unit.Speed = Math.Max(100, unit.Speed + unit.Speed * _speedBonusRatio);
        unit.Damage = Math.Max(10, unit.Damage + (int) (unit.Damage * _damageBonusRatio));
    }

    private void ShowEffect(Unit unit)
    {
        var floatingText = (FloatingText) _floatingTextPrefab.Instance();
        floatingText.Text = _effectText;
        floatingText.Color = _effectTextColor;
        unit.AddChild(floatingText);
    }

    private void OnDestinationReached()
    {
        _potionObject.Position = Destination;
        _potionObject.ZIndex = 0;
        _sprite.Scale = Vector2.One;
        Destination = Vector2.Zero;
        _area.Monitoring = true;
    }
}
