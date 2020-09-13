using Godot;

public class Sword : StaticBody2D
{
    public Player player;
    public int damage;

    public void OnEnter(Node other)
    {
        if (other is Monster monster)
        {
            monster.OnHit(player, damage);
        }
    }
}
