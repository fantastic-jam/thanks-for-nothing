using System;
using Godot;

public class Monster : KinematicBody2D
{
    [Export] private float _speed = 125.0f;

    public Node2D Target;
    public event Action OnDeath;

    public override void _PhysicsProcess(float delta)
    {
        ZIndex = (int) Position.y;
        if (Target != null)
            MoveAndSlide(Position.DirectionTo(Target.Position) * _speed);
    }

    public void OnHit()
    {
        QueueFree();
        OnDeath?.Invoke();
    }
}
