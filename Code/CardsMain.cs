using Godot;
using System;
using System.Collections.Generic;

public partial class CardsMain : Node2D
{
	private Card _card;
	private Sprite2D _testCard;
	private Label _positionLabel;
	private Label _rotationLabel;
	private Label _velocityLabel;

	private Dealer _dealer;

	private CardTableBox _box1;
	private CardTableBox _box2;
	private CardTableBox _box3;
	private CardTableBox _box4;
	private CardTableBox _box5;
	private CardTableBox _box6;
	private CardTableBox _box7;
	private CardTableBox _box8;
	private CardTableBox _box9;
	private CardTableBox _box10;

	private List<CardTableBox> _tableBoxes;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_dealer = GetNode<Dealer>("Dealer");
		_card = GetNode<Card>("Card");
		_positionLabel = GetNode<Label>("PositionContainer/CardPositionValueLabel");
		_rotationLabel = GetNode<Label>("RotationContainer/CardRotationValueLabel");
		_velocityLabel = GetNode<Label>("VelocityContainer/CardVelocityValueLabel");
		
		InitializeCardBoxes();

		_dealer.OnDealRequested += OnDealRequested;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_positionLabel.Text = _card.GlobalPosition.ToString("0.0");
		_rotationLabel.Text = _card.GlobalRotation.ToString("0.0");
		_velocityLabel.Text = _card.Velocity.ToString();
	}

	private void InitializeCardBoxes()
	{ 
		_box1 = GetNode<CardTableBox>("TableBoxes/CardTableBox1");
		_box2 = GetNode<CardTableBox>("TableBoxes/CardTableBox2");
        _box3 = GetNode<CardTableBox>("TableBoxes/CardTableBox3");
        _box4 = GetNode<CardTableBox>("TableBoxes/CardTableBox4");
        _box5 = GetNode<CardTableBox>("TableBoxes/CardTableBox5");
        _box6 = GetNode<CardTableBox>("TableBoxes/CardTableBox6");
        _box7 = GetNode<CardTableBox>("TableBoxes/CardTableBox7");
        _box8 = GetNode<CardTableBox>("TableBoxes/CardTableBox8");
        _box9 = GetNode<CardTableBox>("TableBoxes/CardTableBox9");
        _box10 = GetNode<CardTableBox>("TableBoxes/CardTableBox10");

		_tableBoxes = new List<CardTableBox>
		{
            _box1,
            _box2,
            _box3,
            _box4,
            _box5,
            _box6,
            _box7,
            _box8,
            _box9,
            _box10
        };
	}

    /// <summary>
    /// This is temporary as when dealing we need to place on the table
    /// before putting them into the table boxes
    /// </summary>
    /// <returns>Returns the position of the next fillable box.</returns>
    public void OnDealRequested()
	{
		var firstFillableBox = GetFirstBoxThatIsFillable();
		var boxPosition = firstFillableBox.GlobalPosition;

		if (boxPosition != _dealer.TargetDealPosition) 
		{
			_dealer.TargetDealPosition = boxPosition;	
		}		
	}

	private CardTableBox GetFirstBoxThatIsFillable()
	{
		for (int i = 0; i < _tableBoxes.Count; i++)
		{
			if (!_tableBoxes[i].IsBoxFull)
			{
				return _tableBoxes[i];
			}
		}

		return null;
	}
}
