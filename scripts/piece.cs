using Godot;
using System;
using System.Collections.Generic;
using System.Data.Common;

public partial class piece : Node2D
{
	[Export] private Vector2 origin;
	private Vector2 dir;
	private Timer gt, dt, ldt;
	public bool isPlayable = true;
	public bool isGhost = false;
	private Vector2[] defaultBlockPositions = new Vector2[4];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for(int i=0; i<4; i++)
		{
			defaultBlockPositions[i] = GetChild(i).GetNode<Sprite2D>(".").Position;
		}

		if(isPlayable)
		{
			gt = GetNode<Timer>("../GravityTimer");
			gt.WaitTime = game.frameTime*game.gravity;
			gt.Timeout += Fall;

			dt = GetNode<Timer>("../DASTimer");
			dt.WaitTime = game.frameTime*game.ARR;
			dt.Timeout += DASMove;

			ldt = GetNode<Timer>("../LockDelayTimer");
			ldt.WaitTime = game.frameTime*game.lockDelay;
			ldt.Timeout += checkLock;

			gt.Start();

			dir = game.ARRDir;

			// ghost
			var ghost = Duplicate();
			ghost.GetNode<piece>(".").isPlayable = false;
			ghost.GetNode<piece>(".").isGhost = true;
			ghost.GetNode<Node2D>(".").Position = Position;
			ghost.GetNode<Node2D>(".").Modulate = new Color(0xffffff64);
			ghost.Name = "ghost";
			GetParent().AddChild(ghost);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(isPlayable)
		{
			if (Input.IsActionJustPressed("left"))
			{
				ldt.Stop();
				ldt.Start();
				dir = Vector2.Left;
				DASMove();
				GetNode<Node2D>("../ghost").Position = Position;
				dt.WaitTime = game.frameTime*game.DAS;
				dt.Start();
				dt.WaitTime = game.frameTime*game.ARR;
			}
			if (Input.IsActionJustReleased("left")) dt.Stop();
			if (Input.IsActionJustPressed("right"))
			{
				ldt.Stop();
				ldt.Start();
				dir = Vector2.Right;
				DASMove();
				GetNode<Node2D>("../ghost").Position = Position;
				dt.WaitTime = game.frameTime*game.DAS;
				dt.Start();
				dt.WaitTime = game.frameTime*game.ARR;
			}
			if (Input.IsActionJustReleased("right")) dt.Stop();
			if (Input.IsActionJustPressed("down"))
			{
				Fall();
				gt.Stop();
				gt.WaitTime = game.frameTime*game.gravity/game.SDF;
				gt.Start();
			}
			if (Input.IsActionJustReleased("down"))
			{
				gt.Stop();
				gt.WaitTime = game.frameTime*game.gravity;
				gt.Start();
			}
			if (Input.IsActionJustPressed("rotl"))
			{
				ldt.Stop();
				ldt.Start();

				foreach(var n in GetChildren())
				{
					var sp = (Sprite2D)n;
					Vector2 oldPos = sp.Position-origin;
					sp.Position = new Vector2(oldPos.Y, -oldPos.X)+origin;
				}
				if(!checkMove())
				{
					foreach(var n in GetChildren())
					{
						var sp = (Sprite2D)n;
						Vector2 oldPos = sp.Position-origin;
						sp.Position = new Vector2(-oldPos.Y, oldPos.X)+origin;
					}
				}

				for(int i=0; i<4; i++)
				{
					GetNode<Node2D>("../ghost").GetChild(i).GetNode<Sprite2D>(".").Position = GetChild(i).GetNode<Sprite2D>(".").Position;
				}

				GetNode<Node2D>("../ghost").Position = Position;
			}
			if (Input.IsActionJustPressed("rotr"))
			{
				ldt.Stop();
				ldt.Start();

				foreach(var n in GetChildren())
				{
					var sp = (Sprite2D)n;
					Vector2 oldPos = sp.Position-origin;
					sp.Position = new Vector2(-oldPos.Y, oldPos.X)+origin;
				}
				if(!checkMove())
				{
					foreach(var n in GetChildren())
					{
						var sp = (Sprite2D)n;
						Vector2 oldPos = sp.Position-origin;
						sp.Position = new Vector2(oldPos.Y, -oldPos.X)+origin;
					}
				}

				for(int i=0; i<4; i++)
				{
					GetNode<Node2D>("../ghost").GetChild(i).GetNode<Sprite2D>(".").Position = GetChild(i).GetNode<Sprite2D>(".").Position;
				}

				GetNode<Node2D>("../ghost").Position = Position;
			}
			if (Input.IsActionJustPressed("rot180"))
			{
				ldt.Stop();
				ldt.Start();

				foreach(var n in GetChildren())
				{
					var sp = (Sprite2D)n;
					Vector2 oldPos = sp.Position-origin;
					sp.Position = new Vector2(-oldPos.X, -oldPos.Y)+origin;
				}
				if(!checkMove())
				{
					foreach(var n in GetChildren())
					{
						var sp = (Sprite2D)n;
						Vector2 oldPos = sp.Position-origin;
						sp.Position = new Vector2(-oldPos.X, -oldPos.Y)+origin;
					}
				}

				for(int i=0; i<4; i++)
				{
					GetNode<Node2D>("../ghost").GetChild(i).GetNode<Sprite2D>(".").Position = GetChild(i).GetNode<Sprite2D>(".").Position;
				}

				GetNode<Node2D>("../ghost").Position = Position;
			}
			if (Input.IsActionJustPressed("hold") && game.canHold)
			{
				GetNode("../ghost").QueueFree();
				var holdPiece = GetNode<Node2D>("../HoldPiece");
				game.ARRDir = dir;
				if(holdPiece.GetChildCount()!=0) 
				{
					var newPiece = holdPiece.GetChild(0);
					holdPiece.RemoveChild(newPiece);
					GetParent().AddChild(newPiece);
					newPiece.GetNode<Node2D>(".").Position = game.startPos;
					newPiece.GetNode<piece>(".").isPlayable = true;
					newPiece.GetNode<piece>(".").dir = game.ARRDir;

					var ghost = newPiece.Duplicate();
					ghost.GetNode<piece>(".").isPlayable = false;
					ghost.GetNode<piece>(".").isGhost = true;
					ghost.GetNode<Node2D>(".").Position = game.startPos;
					ghost.GetNode<Node2D>(".").Modulate = new Color(0xffffff64);
					ghost.Name = "ghost";
					GetParent().AddChild(ghost);
				}
				else
				{
					game.pieceSpawned = false;
				}
				Position = Vector2.Zero;
				GetParent().RemoveChild(this);
				holdPiece.AddChild(this);
				for(int i=0; i<4; i++)
				{
					GetChild(i).GetNode<Sprite2D>(".").Position = defaultBlockPositions[i];
				}
				isPlayable = false;
				game.canHold = false;
			}
			if (Input.IsActionJustPressed("up"))
			{
				while(game.pieceSpawned) 
				{
					Fall();
					checkLock();
					ldt.Stop();
				}
			}
			if (!Input.IsAnythingPressed()) dt.Stop();
		}
		if(isGhost)
		{
			Name = "ghost";
			while(checkMove()) Position += Vector2.Down * game.unit;
			if(!checkMove()) Position -= Vector2.Down * game.unit;
		}
	}

	private void Fall()
	{
		if(isPlayable)
		{
			Position += Vector2.Down * game.unit;
			if(!checkMove())
			{
				if(ldt.IsStopped()) ldt.Start();
				Position -= Vector2.Down * game.unit;
			}
		}
	}

	private void DASMove()
	{
		Position += dir*game.unit;
		if(!checkMove()) Position -= dir*game.unit;
		GetNode<Node2D>("../ghost").Position = Position;
	}

	private bool checkMove()
	{
		foreach(var n in GetChildren())
		{
			var spn = (Sprite2D)n;
			if(spn.GlobalPosition.X < game.minX || spn.GlobalPosition.X > game.maxX || spn.GlobalPosition.Y > game.maxY) return false;
			foreach(var b in GetNode<Node2D>("../Stack").GetChildren())
			{
				var spb = (Sprite2D)b;
				if(spn.GlobalPosition==spb.GlobalPosition) return false;
			}
		}
		return true;
	}

	private void checkLine()
	{
		Dictionary<float, int> blockCount = new Dictionary<float,int>();
		List<float> emptyLines = new List<float>();

		foreach(var b in GetNode<Node2D>("../Stack").GetChildren())
		{
			var spb = (Sprite2D)b;

			if(!blockCount.ContainsKey(spb.Position.Y)) blockCount.Add(spb.Position.Y,1);
			else blockCount[spb.Position.Y]++;

			if(blockCount[spb.Position.Y]==10) emptyLines.Add(spb.Position.Y);
		}

		emptyLines.Sort();

		foreach(float line in emptyLines)
		{
			foreach(var b in GetNode<Node2D>("../Stack").GetChildren())
			{
				var spb = (Sprite2D)b;
				if(spb.Position.Y==line) b.QueueFree();
				if(spb.Position.Y<line) spb.Position+=Vector2.Down*game.unit;
			}
		}		
	}

	private void checkLock()
	{
		if(isPlayable)
		{
			Position += Vector2.Down * game.unit;
			if(!checkMove()) Lock();
			else Position -= Vector2.Down * game.unit;
		}
	}

	private void Lock()
	{
		Position -= Vector2.Down * game.unit;
		var stack = GetNode<Node2D>("../Stack");
		foreach(var n in GetChildren())
		{
			var sp = (Sprite2D)n;
			sp.Position = sp.GlobalPosition;
			sp.Scale = Scale;
			RemoveChild(n);
			stack.AddChild(n);
		}
		if(Position==game.startPos) game.gameOver=true;
		checkLine();
		game.pieceSpawned = false;
		game.canHold = true;
		game.ARRDir = dir;
		GetNode<AudioStreamPlayer>("../ClickSound").Play();
		GetNode("../ghost").QueueFree();
		QueueFree();
	}
}
