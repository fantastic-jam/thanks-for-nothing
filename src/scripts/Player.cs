using Godot;

public class Player : Unit
{
    [Export] private PackedScene _swordPrefab = null;
    [Export] private PackedScene _deadPlayerPrefab = null;
    [Export] private int _device = 0;

    private uint _attackCooldown = 300;
    private uint _nextAttackTime = 0;

    public Player()
    {
        LookingToRight = true;
        MaxHealth = 100;
        Health = MaxHealth;
        Damage = 40;
        Speed = 150.0f;
        BumpStrength = 250.0f;
        StunDurationMs = 0;
        OnDeath += DieEffect;
    }

    public override void _Process(float delta)
    {
        HandleInputs();
        base._Process(delta);
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

    private void Attack()
    {
        uint time = OS.GetTicksMsec();
        if (_nextAttackTime > time) return;

        _nextAttackTime = time + _attackCooldown;
        var sword = (Sword) _swordPrefab.Instance();
        sword.Position = _center + _direction * 10;
        sword.Rotation = -_direction.AngleTo(Vector2.Up);
        sword.Fighter = this;
        AddChild(sword);
    }

    private void DieEffect()
    {
        var deadPlayer = (Node2D)_deadPlayerPrefab.Instance();
        deadPlayer.ZIndex = ZIndex;
        deadPlayer.Position = Position;
        deadPlayer.GetNode<Sprite>("Sprite").FlipH = _sprite.FlipH;
        deadPlayer.GetNode<AnimationPlayer>("AnimationPlayer").Autoplay = _sprite.FlipH ? "right death" : "left death";
        _sprite.Visible = false;
        GetParent().AddChild(deadPlayer);
    }
}
