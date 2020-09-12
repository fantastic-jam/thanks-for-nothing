using System;
using Godot;

public class Potion : StaticBody2D
{
    private const float Speed = 300.0f;

    public Vector2 Destination;

    private Area2D _area;
    private Vector2 _origin;
    private float _totalDistance;
    private Sprite _sprite;

    public override void _Ready()
    {
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
            OnDestinationReached();
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
        GD.Print("Drink");
        QueueFree();
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
