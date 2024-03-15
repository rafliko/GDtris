using Godot;
using System;

public partial class game : Node2D
{
	public static float frameTime = 1f/60f;
	public static float unit = 28f;
	public static float gravity = 1f/30f;
	public static float ARR = 1f/2f;
	public static float DAS = 1f/10f;
	public static float SDF = 10f;

	public pieceType[] queue = new pieceType[7];
	[Export] private PackedScene[] pieces;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		randomizers.bagGenerator(queue);
		SpawnPiece(queue[0]);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void SpawnPiece(pieceType type)
	{
		var obj = (Node2D)pieces[(int)type].Instantiate();
		obj.Position = new Vector2(584f,80f);
		AddChild(obj);
	}

	public enum pieceType
	{
		I, J, L, O, S, T, Z
	}
}
