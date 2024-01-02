using Godot;
using System;

public partial class TestCardGen : Node2D
{
    public PackedScene CardScene { get; set; }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		CardScene = ResourceLoader.Load<PackedScene>("res://Scenes/Card.tscn");

		var card = CardScene.Instantiate<Card>();
		this.AddChild(card); // Card needs to be added to tree to trigger Ready function for instantiation

		var texture = GD.Load<Texture2D>("res://Assets/Cards/md8.png");

        card.ChangeTexture(texture);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
