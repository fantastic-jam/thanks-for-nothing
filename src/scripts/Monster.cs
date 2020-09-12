using Godot;

public class Monster : KinematicBody2D
{
    public override void _Process(float delta)
    {
        ZIndex = (int)Position.y;
    }

    public void OnHit()
    {
        GD.Print("Ouch!");
    }
}
