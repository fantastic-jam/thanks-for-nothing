using System.Threading;
using Godot;

public class GameManager : Node
{
    [Export] private int _maxMonsterCount = 1;
    [Export] private PackedScene _playerPrefab = null;
    [Export] private PackedScene _monsterPrefab = null;

    private Node2D _playerSpawner;
    private Node2D _monsterSpawner;
    private int _monsterCount;
    private Player _player;

    public override void _Ready()
    {
        _monsterSpawner = GetNode<Node2D>("MonsterSpawner");
        _playerSpawner = GetNode<Node2D>("PlayerSpawner");
        _player = SpawnPlayer();
        SpawnMonster();
    }

    private Player SpawnPlayer()
    {
        var player = (Player) _playerPrefab.Instance();
        player.Position = _playerSpawner.Position;
        AddChild(player);
        return player;
    }

    private void SpawnMonster()
    {
        var monster = (Monster) _monsterPrefab.Instance();
        Interlocked.Increment(ref _monsterCount);
        monster.Target = _player;
        monster.Position = _monsterSpawner.Position;
        monster.OnDeath += OnMonsterDeath;
        AddChild(monster);
    }

    private void OnMonsterDeath()
    {
        if (Interlocked.Decrement(ref _monsterCount) < _maxMonsterCount)
        {
            SpawnMonster();
        }
    }
}
