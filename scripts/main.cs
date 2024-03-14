using Godot;
using System;

public partial class main : Node2D
{
	public pieceType[] queue = new pieceType[7];
	private float unit = 28f;
	[Export] private PackedScene[] pieces;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		randomizers.randomGenerator(queue);
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
