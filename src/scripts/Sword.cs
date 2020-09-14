using Godot;

public class Sword : StaticBody2D
{
    public Unit Fighter { get; set; }

    public void OnEnter(Node other)
    {
        if (other is Monster monster)
            Fighter.Hit(monster);
    }
}
