using System;
using Godot;

public class DropZone : Area2D
{
    public Rect2 Zone { get; private set; }

    public override void _Ready()
    {
        if (!(GetNode<CollisionShape2D>("CollisionShape2D").Shape is RectangleShape2D shape))
            throw new MissingFieldException("DropZone should have a CollisionShape2D with a RectangleShape2D");
        Zone = new Rect2(
            new Vector2(Position.x - shape.Extents.x, Position.y - shape.Extents.y),
            new Vector2(shape.Extents.x * 2, shape.Extents.y * 2)
        );
    }
}
