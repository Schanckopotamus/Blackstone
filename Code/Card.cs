using Blackstone.Code;
using Godot;
using System;

public partial class Card : Area2D
{
	private float _radiansPerRotation = 6.28318f;
	//private float _rotationPerSecond;
	private Rect2 _windowSize;

	public int ModeganCardValue { get; set; }
	private Sprite2D _cardSprite;

    [Export]
	public float RotationPerSecond { get; set; }

	// Movement per second
	//[Export]
	public int Speed { get; set; }
	public Vector2 Velocity { get; set; }

	// NOTE: Should probably be internally controlled in the future
	public bool isDealt = false;

    private Label _positionLabel;
	private Label _gPositionLabel;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		//_rotationPerSecond = 0.25f;
		_windowSize = GetViewport().GetVisibleRect(); 
		//this.Position = new Vector2(_windowSize.Size.X / 2, _windowSize.Size.Y / 2);
		// Start by moving to the right
		Velocity = Vector2.Zero;//= new Vector2(1, 0);
		Speed = 0;

		_cardSprite = GetNode<Sprite2D>("Sprite2D");

		_positionLabel = GetNode<Label>("PositionLabel/PositionValue");
        _gPositionLabel = GetNode<Label>("GPositionLabel/GPositionValue");

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var floatDelta = (float)delta;

		if (isDealt) 
		{
            ProcessMovement(floatDelta);
		    ProcessRotation(floatDelta);
        }


        _positionLabel.Text = this.Position.ToString();
        _gPositionLabel.Text = this.GlobalPosition.ToString();

        //ProcessSpeed(floatDelta);		
    }

	public void ChangeTexture(Texture2D texture)
	{
		_cardSprite.Texture = texture;
	}

	/// <summary>
	/// Makes sure the card scene doesn't rotate, is not being dealt, no speed & no velocity (direction)
	/// </summary>
	public void SetToLayFlat()
	{ 
		this.isDealt = false;
		this.Velocity = Vector2.Zero;
		this.Speed = 0;
		this.Rotation = 0;
		this.Visible = true;
	}

	/// <summary>
	/// Same functionality as 'SetToLayFlat' but in addition sets position
	/// </summary>
	/// <param name="position">Vector2 Position</param>
	/// <param name="isGlobal">Indicates if position should be set to global parameter or not</param>
    public void SetToLayFlatAt(Vector2 position, bool isGlobal = false)
    {
		this.SetToLayFlat();

		if (isGlobal)
		{
			this.GlobalPosition = position;
			this.Position = Vector2.Zero;
		}
		else 
		{
			this.Position = position;
		}
    }

    private void ProcessMovement(float delta)
	{
		var distance = Velocity.Normalized() * Speed;

		Position += distance * delta;
	}

	private void ProcessSpeed(float delta)
	{
		if (GlobalPosition.X > _windowSize.Size.X 
			&& Velocity.X == 1) 
		{
            Velocity = Velocity * -1;
		}

        if (GlobalPosition.X < 0 
			&& Velocity.X == -1)
        {
            Velocity = Velocity * -1;
        }

        var distance = Velocity.Normalized() * Speed;

		Position += distance * delta;
	}

	private void ProcessRotation(float delta)
	{ 
		float radiansPerFrame = _radiansPerRotation * delta * RotationPerSecond;

		GlobalRotation += radiansPerFrame;
	}
}
