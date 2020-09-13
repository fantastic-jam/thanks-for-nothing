using System.Threading;
using Godot;
using Timer = Godot.Timer;

public class GameManager : Node
{
    private readonly RandomNumberGenerator _rand = new RandomNumberGenerator();

    [Export] private int _maxMonsterCount = 10;
    [Export] private PackedScene _playerPrefab = null;
    [Export] private PackedScene _monsterPrefab = null;

    private Node2D _playerSpawner;
    private Node2D _monsterSpawner;
    private Player _player;
    private Timer _monsterSpawnTimer;
    private HUD _hud;

    private int _monsterCount;
    private int _monsterKill;

    public override void _Ready()
    {
        _rand.Randomize();
        _hud = GetNode<HUD>("HUD");
        _monsterSpawner = GetNode<Node2D>("MonsterSpawner");
        _playerSpawner = GetNode<Node2D>("PlayerSpawner");
        _player = SpawnPlayer();
        _monsterSpawnTimer = new Timer
        {
            OneShot = false,
            Autostart = true,
            WaitTime = 2f
        };
        _monsterSpawnTimer.Connect("timeout", this, nameof(SpawnMonster));
        AddChild(_monsterSpawnTimer);
    }

    public override void _Process(float delta)
    {
        _hud.MonsterKill = _monsterKill;
        _hud.MaxHealth = _player.MaxHealth;
        _hud.Damage = _player.Damage;
        _hud.Speed = (int)_player.Speed;
    }

    private Player SpawnPlayer()
    {
        var player = (Player) _playerPrefab.Instance();
        player.Position = _playerSpawner.Position;
        player.OnDeath += OnPlayerDeath;
        AddChild(player);
        return player;
    }

    private void SpawnMonster()
    {
        if (_monsterCount >= _maxMonsterCount) return;
        var monster = (Monster) _monsterPrefab.Instance();
        Interlocked.Increment(ref _monsterCount);
        monster.Target = _player;
        monster.Position = _monsterSpawner.Position + new Vector2(_rand.RandiRange(-120, 120), 0.0f);
        monster.Damage += _monsterKill / 10;
        monster.MaxHealth += _monsterKill;
        monster.OnDeath += OnMonsterDeath;
        AddChild(monster);
    }

    private void OnMonsterDeath()
    {
        Interlocked.Decrement(ref _monsterCount);
        _monsterKill++;
    }

    private void OnPlayerDeath()
    {
        _monsterSpawnTimer.Stop();
        var timer = new Timer
        {
            OneShot = true,
            Autostart = true,
            WaitTime = 5
        };
        timer.Connect("timeout", this, nameof(ReloadGame));
        AddChild(timer);
    }

    private void ReloadGame()
    {
        GetTree().ReloadCurrentScene();
    }
}
