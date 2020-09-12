using Godot;

public class Monster : KinematicBody2D
{
    [Export] private float _speed = 125.0f;

    public override void _PhysicsProcess(float delta)
    {
        ZIndex = (int)Position.y;
        if (Owner.FindNode("Player") is Player player)
        {
            MoveAndSlide(Position.DirectionTo(player.Position) * _speed);
        }
    }

    public void OnHit()
    {
        GD.Print("Ouch!");
    }
}
