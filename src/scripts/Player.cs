using System;
using Godot;

public class Player : KinematicBody2D
{
    [Export] private PackedScene _swordPrefab = null;
    [Export] private int _device = 0;

    private uint _attackCooldown = 300;
    public int MaxHealth = 100;
    public int Health;
    public int Damage = 25;
    public float Speed = 150.0f;

    public event Action OnDeath;

    private Sprite _sprite;
    private ProgressBar _healthBar;
    private Vector2 _center = Vector2.Zero;
    private Vector2 _size = Vector2.Zero;
    private Vector2 _direction = Vector2.Up;
    private Vector2 _velocity = Vector2.Zero;
    private uint _nextAttackTime = 0;

    public override void _Ready()
    {
        Health = MaxHealth;
        _sprite = GetNode<Sprite>("Sprite");
        _size = _sprite.RegionRect.Size;
        _center = new Vector2(_size.x / 2, _size.y / 2);
        _healthBar = (ProgressBar) FindNode("HealthBar");
    }

    public override void _Process(float delta)
    {
        ZIndex = (int) Position.y;
        UpdateGUI();
        HandleInputs();
        HandleBody();
        Update(); // Force redraw
    }

    private void UpdateGUI()
    {
        _healthBar.MaxValue = MaxHealth;
        _healthBar.Value = Health;
    }


    private void HandleInputs()
    {
        if (Input.IsActionJustPressed($"attack_{_device}"))
        {
            Attack();
        }

        _velocity = new Vector2(
            Input.GetActionStrength($"right_{_device}") - Input.GetActionStrength($"left_{_device}"),
            Input.GetActionStrength($"down_{_device}") - Input.GetActionStrength($"up_{_device}")
        ).Normalized() * Speed;
    }

    private void HandleBody()
    {
        if (_velocity != Vector2.Zero)
        {
            _direction = _velocity.Normalized();
            _sprite.FlipH = _velocity.x < 0;
        }
    }

    private void Attack()
    {
        uint time = OS.GetTicksMsec();
        if (_nextAttackTime > time) return;

        _nextAttackTime = time + _attackCooldown;
        var sword = (Sword) _swordPrefab.Instance();
        sword.Position = _center + _direction * 10;
        sword.Rotation = -_direction.AngleTo(Vector2.Up);
        sword.player = this;
        sword.damage = Damage;
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

    public void OnHit(Monster monster)
    {
        MoveAndSlide(monster.Position.DirectionTo(Position) * 250.0f);
        Health -= 2;
        if (Health < 1)
        {
            OnDeath?.Invoke();
            QueueFree();
        }
    }
}
