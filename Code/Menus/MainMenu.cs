using Godot;
using System;

public partial class MainMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartButtonPressed()
	{
		this.GetTree().ChangeSceneToFile("res://Scenes/CardsMain.tscn");
	}

	public void OnRulesButtonPressed()
	{
        this.GetTree().ChangeSceneToFile("res://Scenes/RulesMenu.tscn");
    }

	public void QuitButtonPressed()
	{
		this.GetTree().Quit();
	}
}
