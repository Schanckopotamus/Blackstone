using Blackstone.Code;
using Blackstone.Code.Buses;
using Blackstone.Code.Enums;
using Godot;
using System;

public partial class Card : Area2D
{
	private float _radiansPerRotation = 6.28318f;
	//private float _rotationPerSecond;
	private Rect2 _windowSize;

	[Export]
	public int ModeganCardValue { get; set; }
	private Sprite2D _cardSprite;

    public bool IsBlackstone 
	{
		get
		{
			return ModeganCardValue == 10;
		}
	}
	public bool IsWhitestone 
	{
		get
		{
			return !IsBlackstone && ModeganCardValue != 0;
		}
	}

    [Export]
	public float RotationPerSecond { get; set; }

	[Export]
	public Texture2D CardTexture { get; set; }

	// Movement per second
	//[Export]
	public int Speed { get; set; }
	public Vector2 Velocity { get; set; }

	// NOTE: Should probably be internally controlled in the future
	public bool isDealt = false;

    private Label _positionLabel;
	private Label _gPositionLabel;

	private CollisionOrchestrator _collisionOrchestrator;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _collisionOrchestrator = GetNode<CollisionOrchestrator>("/root/CollisionOrchestrator");
        //_rotationPerSecond = 0.25f;
        _windowSize = GetViewport().GetVisibleRect(); 
		//this.Position = new Vector2(_windowSize.Size.X / 2, _windowSize.Size.Y / 2);
		// Start by moving to the right
		Velocity = Vector2.Zero;//= new Vector2(1, 0);
		Speed = 0;

		_cardSprite = GetNode<Sprite2D>("Sprite2D");
		if (CardTexture != null) 
		{
			_cardSprite.Texture = CardTexture;
		}

		_positionLabel = GetNode<Label>("PositionLabel/PositionValue");
        _gPositionLabel = GetNode<Label>("GPositionLabel/GPositionValue");

		// When you need to know when a Transform property changes
		//this.SetNotifyTransform(true);
    }

    public override void _Notification(int what)
    {
		if (what == NotificationTransformChanged) 
		{
			GD.Print("*** Card Transform Changed ***");
		}
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
	/// Set necessary fields for movement.
	/// </summary>
	/// <param name="velocity">Direction to be delt, should be normalized but will do so if not</param>
	/// <param name="speed">Pixels per second we want the movement to be</param>
	public void SetToDealt(Vector2 velocity, int speed, DealTarget dealTarget)
	{ 
		if (velocity == Vector2.Zero) 
		{
			SetToLayFlat();
		}
		else
		{
			_collisionOrchestrator.ChangeDealingTarget(dealTarget);

			if (!velocity.IsNormalized())
			{
				velocity = velocity.Normalized();
			}

			this.isDealt = true;
			this.Velocity = velocity;
			this.Speed = speed;
			this.Rotation = this.RotationPerSecond;
			this.Visible = true;
		}		 
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

	private void HandleCardExitedScreen()
	{
		this.QueueFree();
	}
}
