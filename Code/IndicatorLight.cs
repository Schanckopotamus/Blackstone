using Godot;
using System;

public partial class IndicatorLight : Node2D
{
	private Sprite2D _redLight;
	private Sprite2D _greenLight;
	private Label _lightLabel;

	private bool _indicator;

	[Export]
	public string IndicatorText { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_indicator = false;

		_redLight = GetNode<Sprite2D>("RedLight");
        _greenLight = GetNode<Sprite2D>("GreenLight");
		_lightLabel = GetNode<Label>("Label");
		_lightLabel.Text = IndicatorText;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_indicator) 
		{
			_redLight.Visible = false;
		}
		else
		{
			_redLight.Visible = true;
		}
	}

	public void SetIndicator(bool indicator)
	{ 
		_indicator = indicator;
	}

	public void SetLabel(string labelText)
	{ 
		_lightLabel.Text = labelText;
	}
}
