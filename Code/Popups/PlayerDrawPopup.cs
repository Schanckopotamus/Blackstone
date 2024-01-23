using Godot;
using System;

public partial class PlayerDrawPopup : Control
{
	private Label _playerNameLabel;
	private HSlider _drawSlider;
	private Label _drawCountLabel;

	private Button _foldButton;
	private Button _drawButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_playerNameLabel = GetNode<Label>("PopupPanel/VBoxContainer/PlayerNameLabel");
		_playerNameLabel.Text = "Player 1";
		_drawSlider = GetNode<HSlider>("PopupPanel/VBoxContainer/DrawSlider");
		_drawCountLabel = GetNode<Label>("PopupPanel/VBoxContainer/ValueLabel");

		_drawSlider.MaxValue = 5;
		_drawSlider.TickCount = 5;
		_drawCountLabel.Text = _drawSlider.MinValue.ToString();

		_foldButton = GetNode<Button>("PopupPanel/VBoxContainer/HBoxContainer/FoldButton");
        _drawButton = GetNode<Button>("PopupPanel/VBoxContainer/HBoxContainer/DrawButton");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void HandleFoldButtonPressed()
	{ 
		// Emit Signal
	}

	private void HandleDrawButtonPressed() 
	{
		// Emit Signal
	}

	private void HandleSliderValueChanged(float newValue) 
	{
        _drawCountLabel.Text = newValue.ToString();
    }
}
