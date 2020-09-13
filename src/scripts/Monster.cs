using System;
using Godot;

public class Monster : KinematicBody2D
{
    [Export] private float _speed = 125.0f;

    public int MaxHealth = 100;
    public int Health = 100;
    public int Damage = 5;

    public Player Target;
    public event Action OnDeath;

    private ProgressBar _healthBar;
    private Vector2 _velocity = Vector2.Zero;
    private Sprite _sprite;
    private uint _stunUntil = 0;

    public override void _Ready()
    {
        Health = MaxHealth;
        _sprite = GetNode<Sprite>("Sprite");
        _healthBar = (ProgressBar) FindNode("HealthBar");
        Target.OnDeath += () => Target = null;
    }

    public override void _Process(float delta)
    {
        UpdateGUI();
        HandleBody();
        if (Target != null && OS.GetTicksMsec() > _stunUntil)
        {
            _velocity = Position.DirectionTo(Target.Position) * _speed;
            for (var i = 0; i < GetSlideCount(); i++)
                ApplyCollision(GetSlideCollision(i));
        }
        else
        {
            _velocity = Vector2.Zero;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        ZIndex = (int) Position.y;
        MoveAndSlide(_velocity);
    }

    private void ApplyCollision(KinematicCollision2D collision)
    {
        if (collision.Collider is Player player)
        {
            Vector2 bump = player.Position.DirectionTo(Position) * 500.0f;
            player.OnHit(this, Damage);
            MoveAndSlide(bump);
        }
    }

    private void HandleBody()
    {
        if (_velocity != Vector2.Zero)
        {
            _sprite.FlipH = _velocity.x > 0;
        }
    }

    private void UpdateGUI()
    {
        _healthBar.MaxValue = MaxHealth;
        _healthBar.Value = Health;
    }

    public void OnHit(Player player, int damage)
    {
        MoveAndSlide(player.Position.DirectionTo(Position) * 500.0f);
        Health -= damage;
        if (Health < 1)
        {
            QueueFree();
            OnDeath?.Invoke();
            return;
        }
        _stunUntil = OS.GetTicksMsec() + 500;
    }
}
