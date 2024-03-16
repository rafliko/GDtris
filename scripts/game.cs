using Godot;
using System;
using System.Collections.Generic;

public partial class game : Node2D
{
	public static float frameTime = 1f/60f;
	public static float unit = 28f;
	public static float gravity = 30f;
	public static float ARR = 2f;
	public static float DAS = 10f;
	public static float SDF = 10f;
	public static float lockDelay = 30f;
	public static float minX = 500f;
	public static float maxX = 752f;
	public static float maxY = 612f;
	public static bool pieceSpawned = false;
	public static bool gameOver = false;
	public static bool canHold = true;
	public static Vector2 ARRDir = Vector2.Zero;
	public static Vector2 startPos = new Vector2(584f,52f);
	
	private List<pieceType> queue = new List<pieceType>();
	[Export] private PackedScene[] pieces;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		randomizers.bagGenerator(queue);
		SpawnPiece(queue[0]);
		queue.RemoveAt(0);
		var nextPiece = (Node2D)pieces[(int)queue[0]].Instantiate();
		nextPiece.GetNode<piece>(".").isPlayable = false;
		GetNode<Node2D>("Queue").AddChild(nextPiece);
		pieceSpawned = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!pieceSpawned)
		{
			SpawnPiece(queue[0]);
			queue.RemoveAt(0);
			if(queue.Count == 0) randomizers.bagGenerator(queue);
			GetNode<Node2D>("Queue").GetChild(0).QueueFree();
			var nextPiece = (Node2D)pieces[(int)queue[0]].Instantiate();
			nextPiece.GetNode<piece>(".").isPlayable = false;
			GetNode<Node2D>("Queue").AddChild(nextPiece);
			pieceSpawned = true;
		}

		if(gameOver)
		{
			gameOver = false;
			GetTree().ChangeSceneToFile("res://scenes/game.tscn");
		}
	}

	public void SpawnPiece(pieceType type)
	{
		var obj = (Node2D)pieces[(int)type].Instantiate();
		obj.Position = startPos;
		AddChild(obj);
	}

	public enum pieceType
	{
		I, J, L, O, S, T, Z
	}
}
