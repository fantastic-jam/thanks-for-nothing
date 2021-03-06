using Godot;
using System;

public class Unit : KinematicBody2D
{
    public int MaxHealth { get; set; }
    public int Health { get; set; }
    public int Damage { get; set; }
    public float Speed { get; set; }
    public float BumpStrength { get; set; }
    public uint StunDurationMs { get; set; }
    public bool LookingToRight { get; set; }
    public string IdleAnimation { get; set; }

    [Export] private AudioStream _hurtSound;

    public event Action OnDeath;

    protected Sprite _sprite;
    protected ProgressBar _healthBar;
    protected AnimationPlayer _animationPlayer;
    protected AudioStreamPlayer2D _soundPlayer;
    protected Vector2 _center = Vector2.Zero;
    protected Vector2 _size = Vector2.Zero;
    protected Vector2 _direction = Vector2.Up;
    protected Vector2 _velocity = Vector2.Zero;
    protected uint _stunUntil = 0;
    protected bool _isDead;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite>("Sprite");
        _healthBar = (ProgressBar) FindNode("HealthBar");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _soundPlayer = GetNode<AudioStreamPlayer2D>("SoundPlayer");
        _size = _sprite.RegionRect.Size;
        _center = new Vector2(_size.x / 2, _size.y / 2);
    }

    public override void _Process(float delta)
    {
        ZIndex = (int) Position.y;
        UpdateGUI();
        HandleBody();
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_isDead) return;
        _velocity = MoveAndSlide(_velocity, Vector2.Up);
    }

    protected void UpdateGUI()
    {
        _healthBar.MaxValue = MaxHealth;
        _healthBar.Value = Health;
    }

    protected void HandleBody()
    {
        if (_isDead) return;

        if (_velocity != Vector2.Zero)
        {
            _animationPlayer.Play("walk");
            _direction = _velocity.Normalized();
            _sprite.FlipH = _velocity.x < 0 ? LookingToRight : !LookingToRight;
        }
        else if (IdleAnimation != null)
        {
            _animationPlayer.Play(IdleAnimation);
        }
        else
        {
            _animationPlayer.Stop();
        }
    }

    public void Hit(Unit target)
    {
        if (_isDead) return;
        target.OnHit(this, Damage);
    }

    public void OnHit(Unit attacker, int damage)
    {
        if (_isDead) return;

        if (_hurtSound != null)
        {
            _soundPlayer.Stream = _hurtSound;
            _soundPlayer.Play();
        }

        MoveAndSlide(attacker.Position.DirectionTo(Position) * BumpStrength);
        Health -= damage;
        if (Health < 1)
        {
            Die();
            return;
        }

        if (StunDurationMs > 0)
            _stunUntil = OS.GetTicksMsec() + StunDurationMs;
    }

    protected async void Die()
    {
        _isDead = true;
        Visible = false;
        OnDeath?.Invoke();
        await ToSignal(_soundPlayer, "finished");
        QueueFree();
    }
}
