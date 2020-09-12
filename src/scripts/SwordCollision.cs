using Godot;

public class SwordCollision : Area2D
{
    public void OnEnter(Node other)
    {
        if (other is Monster monster)
        {
            monster.OnHit();
        }
    }
}
