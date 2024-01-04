using Godot;
using System;

public partial class PlayerScene : Node2D
{
	private Sprite2D _defaultPlayerImage;
    private Sprite2D _activePlayerImage;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_defaultPlayerImage = GetNode<Sprite2D>("DefaultPlayerImage");
		//var halvedImageSize = (_defaultPlayerImage.Texture.GetSize() / 2);
		//var shiftLeftDistance = -1 * halvedImageSize.X / 2;
		//var shiftVector = new Vector2(shiftLeftDistance, 0);

		_activePlayerImage = GetNode<Sprite2D>("ActivePlayerImage");

		this.SetToPassive();

		// to see in window when runnig scene only for testing
		//this.GlobalPosition = new Vector2(1000, 500);
		//this.Scale = new Vector2(0.15f, 0.15f);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetToActive()
	{ 
		_defaultPlayerImage.Visible = false;
		_activePlayerImage.Visible= true;
	}

	public void SetToPassive()
	{ 
		_defaultPlayerImage.Visible = true;
		_activePlayerImage.Visible= false;	
	}
}
