using System;
using Godot;

public class Monster : Unit
{
    public Player Target;

    public Monster()
    {
        LookingToRight = false;
        MaxHealth = 100;
        Health = MaxHealth;
        Damage = 2;
        Speed = 125.0f;
        StunDurationMs = 500;
        BumpStrength = 500.0f;
    }

    public override void _Ready()
    {
        base._Ready();
        Target.OnDeath += Celebrate;
    }

    public override void _Process(float delta)
    {
        if (Target != null && OS.GetTicksMsec() > _stunUntil)
        {
            _velocity = Position.DirectionTo(Target.Position) * Speed;
            Vector2 bump = Vector2.Zero;
            for (var i = 0; i < GetSlideCount(); i++)
                bump = ApplyCollision(GetSlideCollision(i));
            if (bump != Vector2.Zero)
                MoveAndSlide(bump);
        }
        else
        {
            _velocity = Vector2.Zero;
        }

        base._Process(delta);
    }

    private Vector2 ApplyCollision(KinematicCollision2D collision)
    {
        if (!(collision.Collider is Player player))
            return Vector2.Zero;
        Vector2 bump = player.Position.DirectionTo(Position) * BumpStrength;
        Hit(player);
        return bump;
    }

    private void Celebrate()
    {
        Target = null;
        IdleAnimation = "celebrate";
    }
}
