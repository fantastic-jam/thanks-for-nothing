using System;
using Godot;

public class Wizzard : Sprite
{
    private readonly RandomNumberGenerator _rand = new RandomNumberGenerator();

    [Export] private float _dropIntervalInSec = 6.0f;
    [Export] private PackedScene[] _potionPrefabs;

    private Arena _arena;
    private Node _scene;
    private Timer _timer;

    public override void _Ready()
    {
        _rand.Randomize();
        _scene = GetTree().CurrentScene;
        _arena = _scene.FindNode("Arena", true, false) as Arena;
        _timer = new Timer
        {
            WaitTime = _dropIntervalInSec,
            OneShot = false
        };
        _timer.Connect("timeout", this, nameof(ProvideHelp));
        AddChild(_timer);
        _timer.Start();
    }

    public void Stop()
    {
        _timer.OneShot = true;
    }

    private void ProvideHelp()
    {
        var location = new Vector2(
            _rand.RandfRange(_arena.Zone.Position.x, _arena.Zone.End.x),
            _rand.RandfRange(_arena.Zone.Position.y, _arena.Zone.End.y)
        );
        int potionPick = _rand.RandiRange(0, _potionPrefabs.Length - 1);
        var potion = (Potion)_potionPrefabs[potionPick].Instance();
        potion.InitialPosition = GlobalPosition;
        potion.InitialRotation = _rand.Randf() * 2 * (float) Math.PI;
        potion.Destination = location;
        _scene.AddChild(potion);
    }
}
