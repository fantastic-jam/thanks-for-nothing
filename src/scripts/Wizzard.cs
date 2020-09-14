using System;
using Godot;

public class Wizzard : Sprite
{
    private readonly RandomNumberGenerator _rand = new RandomNumberGenerator();

    [Export] private float _dropIntervalInSec = 6.0f;
    [Export] private PackedScene[] _potionPrefabs;

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
        int potionPick = _rand.RandiRange(0, _potionPrefabs.Length - 1);
        var potion = (Potion)_potionPrefabs[potionPick].Instance();
        potion.InitialPosition = GlobalPosition;
        potion.InitialRotation = _rand.Randf() * 2 * (float) Math.PI;
        potion.Destination = location;
        _scene.AddChild(potion);
    }
}
