using System;
using System.Threading;
using Godot;

public class PlayerController : KinematicBody2D
{
	[Export] private PackedScene _swordPrefab = null;
	[Export] private float _speed = 150.0f;
	[Export] private uint _attackCooldown = 300;

	private Sprite _sprite;
	private Vector2 _center = Vector2.Zero;
	private Vector2 _size = Vector2.Zero;
	private Vector2 _direction = Vector2.Up;
	private uint _nextAttackTime = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<Sprite>("Sprite");
		_size = _sprite.RegionRect.Size;
		_center = new Vector2(_size.x / 2, _size.y / 2);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
	}

	public override void _PhysicsProcess(float delta)
	{
		if (Input.IsActionPressed("left_0"))
		{
			_sprite.FlipH = true;
			MoveAndSlide(Vector2.Left * _speed);
			_direction = PlayerDirection();
		}
		else if (Input.IsActionPressed("right_0"))
		{
			_sprite.FlipH = false;
			MoveAndSlide(Vector2.Right * _speed);
			_direction = PlayerDirection();
		}

		if (Input.IsActionPressed("up_0"))
		{
			MoveAndSlide(Vector2.Up * _speed);
			_direction = PlayerDirection();
		}
		else if (Input.IsActionPressed("down_0"))
		{
			MoveAndSlide(Vector2.Down * _speed);
			_direction = PlayerDirection();
		}


		if (Input.IsActionJustPressed("attack_0"))
		{
			uint time = OS.GetTicksMsec();
			if (_nextAttackTime <= time)
			{
				_nextAttackTime = time + _attackCooldown;
				var sword = (StaticBody2D)_swordPrefab.Instance();
				// TODO : Position to after intersection with player
				sword.Position = _center + _direction.Normalized() * 15;
				sword.Rotation = -_direction.AngleTo(Vector2.Up);
				AddChild(sword);
			}
		}

		Update(); // Force redraw
	}

	public override void _Draw()
	{
		DrawLine(_center, _direction.Normalized() * 20, Color.Color8(10, 255, 10));
	}

	private static Vector2 PlayerDirection()
	{
		return new Vector2(Input.GetJoyAxis(0, (int) JoystickList.Axis0), Input.GetJoyAxis(0, (int) JoystickList.Axis1));
	}

}
