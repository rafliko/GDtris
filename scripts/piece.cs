using Godot;
using System;

public partial class piece : Node2D
{
	private Vector2 dir;
	Timer gt, dt;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gt = GetNode<Timer>("../GravityTimer");
		gt.WaitTime = game.frameTime/game.gravity;
		gt.Timeout += Fall;

		dt = GetNode<Timer>("../DASTimer");
		dt.WaitTime = game.frameTime/game.ARR;
		dt.Timeout += DASMove;

		gt.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("left"))
		{
			dir = Vector2.Left;
			DASMove();
			dt.WaitTime = game.frameTime/game.DAS;
			dt.Start();
			dt.WaitTime = game.frameTime/game.ARR;

		}
		if (Input.IsActionJustPressed("right"))
		{
			dir = Vector2.Right;
			DASMove();
			dt.WaitTime = game.frameTime/game.DAS;
			dt.Start();
			dt.WaitTime = game.frameTime/game.ARR;
		}
		if (Input.IsActionJustPressed("down"))
		{
			Fall();
			gt.Stop();
			gt.WaitTime = game.frameTime/game.gravity/game.SDF;
			gt.Start();
		}
		if (Input.IsActionJustReleased("down"))
		{
			gt.Stop();
			gt.WaitTime = game.frameTime/game.gravity;
			gt.Start();
		}
		if (!Input.IsAnythingPressed()) dt.Stop();
	}

	private void Fall()
	{
		Position += Vector2.Down * game.unit;
		if(!checkMove()) Position -= Vector2.Down * game.unit;
	}

	private void DASMove()
	{
		Position += dir*game.unit;
		if(!checkMove()) Position -= dir*game.unit;
	}

	private bool checkMove()
	{
		foreach(Sprite2D n in GetChildren())
		{
			if(n.GlobalPosition.X < 500f || n.GlobalPosition.X > 752f || n.GlobalPosition.Y > 612f) return false;
		}
		return true;
	}
}
