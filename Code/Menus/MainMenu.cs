using Godot;
using System;

public partial class MainMenu : Control
{
	private PackedScene _optionsPackedScene;
	private Node _optionsScene;

    public Button StartButton { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		StartButton = GetNode<Button>("VBoxContainer/Start");
		StartButton.GrabFocus();
		_optionsPackedScene = ResourceLoader.Load<PackedScene>("res://Scenes/OptionsPopup.tscn");

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void StartButtonPressed()
	{
        this.GetTree().ChangeSceneToFile("res://Scenes/CardsMain.tscn");
        //this.GetTree().ChangeSceneToFile("res://Scenes/PlayerSelectionMenu.tscn");
    }

	public void OptionsButtonPressed()
	{
		_optionsScene = _optionsPackedScene.Instantiate();
		this.AddChild(_optionsScene);
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
