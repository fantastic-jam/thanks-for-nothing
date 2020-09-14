using System.Threading;
using Godot;
using Timer = Godot.Timer;

public class GameManager : Node
{
    private readonly RandomNumberGenerator _rand = new RandomNumberGenerator();

    [Export] private int _maxMonsterCount = 10;
    [Export] private PackedScene _playerPrefab = null;
    [Export] private PackedScene _monsterPrefab = null;
    [Export] private PackedScene _gameOver = null;

    private Arena _arena;
    private Node2D _playerSpawner;
    private Player _player;
    private Timer _monsterSpawnTimer;
    private HUD _hud;

    private int _monsterCount;
    private int _monsterKill;
    private bool _isGameOver = false;

    public override void _Ready()
    {
        _rand.Randomize();
        _hud = GetNode<HUD>("HUD");
        _arena = GetNode<Arena>("../Arena");
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

    public override void _Input(InputEvent @event)
    {
        if (_isGameOver)
        {
            switch (@event)
            {
                case InputEventKey _:
                case InputEventMouseButton _:
                case InputEventJoypadButton _:
                    ReloadGame();
                    break;
            }
        }
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
        monster.Position = RandomPositionInArena();
        monster.Damage += _monsterKill / 10;
        monster.MaxHealth += _monsterKill;
        monster.Health = monster.MaxHealth;
        monster.OnDeath += OnMonsterDeath;
        AddChild(monster);
    }

    private Vector2 RandomPositionInArena()
    {
        switch (_rand.RandiRange(0, 3))
        {
            case 0: // top
                return new Vector2(_rand.RandfRange(_arena.Zone.Position.x, _arena.Zone.End.x), _arena.Zone.Position.y);
            case 1: // right
                return new Vector2(_arena.Zone.End.x, _rand.RandfRange(_arena.Zone.Position.y, _arena.Zone.End.y));
            case 2: // bottom
                return new Vector2(_rand.RandfRange(_arena.Zone.Position.x, _arena.Zone.End.x), _arena.Zone.End.y);
            case 3: // left
                return new Vector2(_arena.Zone.Position.x, _rand.RandfRange(_arena.Zone.Position.y, _arena.Zone.End.y));
        }
        return Vector2.Zero;
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
            WaitTime = 2
        };
        timer.Connect("timeout", this, nameof(GameOver));
        AddChild(timer);
    }

    private void GameOver()
    {
        _isGameOver = true;
        AddChild(_gameOver.Instance());
    }

    private void ReloadGame()
    {
        GetTree().ReloadCurrentScene();
    }
}
