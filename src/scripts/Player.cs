using Godot;

public class Player : KinematicBody2D
{
    [Export] private PackedScene _swordPrefab = null;
    [Export] private float _speed = 150.0f;
    [Export] private uint _attackCooldown = 300;
    [Export] private int _device = 0;

    private Sprite _sprite;
    private Vector2 _center = Vector2.Zero;
    private Vector2 _size = Vector2.Zero;
    private Vector2 _direction = Vector2.Up;
    private Vector2 _velocity = Vector2.Zero;
    private uint _nextAttackTime = 0;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite>("Sprite");
        _size = _sprite.RegionRect.Size;
        _center = new Vector2(_size.x / 2, _size.y / 2);
    }

    public override void _Process(float delta)
    {
        ZIndex = (int) Position.y;
        if (Input.IsActionJustPressed($"attack_{_device}"))
        {
            Attack();
        }

        _velocity = new Vector2(
            Input.GetActionStrength($"right_{_device}") - Input.GetActionStrength($"left_{_device}"),
            Input.GetActionStrength($"down_{_device}") - Input.GetActionStrength($"up_{_device}")
        ).Normalized() * _speed;
        if (_velocity != Vector2.Zero)
        {
            _direction = _velocity.Normalized();
            _sprite.FlipH = _velocity.x < 0;
        }

        Update(); // Force redraw
    }

    private void Attack()
    {
        uint time = OS.GetTicksMsec();
        if (_nextAttackTime > time) return;

        _nextAttackTime = time + _attackCooldown;
        var sword = (StaticBody2D) _swordPrefab.Instance();
        sword.Position = _center + _direction * 10;
        sword.Rotation = -_direction.AngleTo(Vector2.Up);
        AddChild(sword);
    }

    public override void _PhysicsProcess(float delta)
    {
        _velocity = MoveAndSlide(_velocity, Vector2.Up);
    }

    public override void _Draw()
    {
        //DrawLine(_center, _center + _velocity, Color.Color8(255, 255, 10));
    }
}
