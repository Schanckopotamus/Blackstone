using Godot;
using System;

public partial class PlayerSelectionMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("Back"))
        {
            OnBackButtonPressed();
        }
    }

	public void OnBackButtonPressed()
	{
        this.GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
    }
}
