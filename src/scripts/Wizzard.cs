using System;
using Godot;

public class Wizzard : Sprite
{
    [Export] private float _dropIntervalInSec = 20.0f;
    [Export] private PackedScene _potionPrefab = null;
    private readonly RandomNumberGenerator _rand = new RandomNumberGenerator();
    private DropZone _dropZone;
    private Node _scene;

    public override void _Ready()
    {
        _rand.Randomize();
        _scene = GetTree().CurrentScene;
        _dropZone = _scene.FindNode("DropZone", true, false) as DropZone;
        var timer = new Timer
        {
            WaitTime = _dropIntervalInSec,
            OneShot = false
        };
        timer.Connect("timeout", this, nameof(ProvideHelp));
        AddChild(timer);
        timer.Start();
    }

    private void ProvideHelp()
    {
        var location = new Vector2(
            _rand.RandfRange(_dropZone.Zone.Position.x, _dropZone.Zone.End.x),
            _rand.RandfRange(_dropZone.Zone.Position.y, _dropZone.Zone.End.y)
        );
        var potion = (Potion)_potionPrefab.Instance();
        potion.Position = GlobalPosition;
        potion.Rotation = _rand.Randf() * 2 * (float)Math.PI;
        potion.Destination = location;
        _scene.AddChild(potion);
    }
}
