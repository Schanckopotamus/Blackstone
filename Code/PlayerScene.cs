using Blackstone.Code.Buses;
using Blackstone.Code.Enums;
using Blackstone.Code.States;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerScene : Node2D
{
	[Signal]
	public delegate void OnCardCollidedEventHandler(PlayerScene box);

	[Export]
	public int SeatPositon { get; set; }

    public int CurrencyAmount 
	{
		get
		{
			return int.TryParse(_currencyValueLabel.Text, out var currencyValue)
				? currencyValue
				: 0;
		}
		set
		{ 
			_currencyValueLabel.Text = value.ToString();
		}
	}

    public Vector2 DealPosition { get; set; }

    // The number of pixels between each card when visible on table.
    private float _cardInHandSpaceing = 35;

	private Sprite2D _defaultPlayerImage;
	private Sprite2D _activePlayerImage;
	// This is for when dealing Blackstones to player.
	private Vector2 _originalGlobalMarkerPosition;
	private Marker2D _dealMarker;
	private Node _cardsInHand;
	private CollisionShape2D _collisionBox;
	private TextureButton _anteButton;
	private SignalBus _signalBus;
	private Label _currencyValueLabel;
	private IndicatorLight _collisionLight;

	private PlayerSeatState _seatState;

	private Label _markerPositionLabel;
	private Label _defaultMarkerPositionLabel;

	public bool IsActive => IsPlayerActive();

	private bool _isAntedIn;
	public bool IsAntedIn
	{
		get => IsPlayerAntedIn();
		set => SetIsAntedIn(value);
	}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _signalBus = GetNode<SignalBus>("/root/SignalBus");

        _defaultPlayerImage = GetNode<Sprite2D>("DefaultPlayerImage");
		//var halvedImageSize = (_defaultPlayerImage.Texture.GetSize() / 2);
		//var shiftLeftDistance = -1 * halvedImageSize.X / 2;
		//var shiftVector = new Vector2(shiftLeftDistance, 0);

		_activePlayerImage = GetNode<Sprite2D>("ActivePlayerImage");

		_dealMarker = GetNode<Marker2D>("CardDealMarker");
		DealPosition = _dealMarker.GlobalPosition;
        _originalGlobalMarkerPosition = _dealMarker.GlobalPosition;

		_cardsInHand = GetNode<Node>("Cards");
		_collisionBox = GetNode<CollisionShape2D>("Box/CollisionShape2D");
		_anteButton = GetNode<TextureButton>("AnteButton");
		_currencyValueLabel = GetNode<Label>("CurrencyContainer/CurrencyAmount");
		_collisionLight = GetNode<IndicatorLight>("CollisionLight");

		_markerPositionLabel = GetNode<Label>("MarkerPositionContainer/Value");
        _defaultMarkerPositionLabel = GetNode<Label>("DefaultMarkerPositionContainer/Value");

        _anteButton.Pressed += HandlePlayeredAnted;

		CurrencyAmount = 100;


		// Makes Player outline sprite active.
		this.SetToPassive();

		SetAnteButtonVisibility(false);



		// to see in window when runnig scene only for testing
		//this.GlobalPosition = new Vector2(1000, 500);
		//this.Scale = new Vector2(0.15f, 0.15f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_collisionLight.SetIndicator(!_collisionBox.Disabled);

		_markerPositionLabel.Text = DealPosition.ToString("0.00");
		_defaultMarkerPositionLabel.Text = _originalGlobalMarkerPosition.ToString("0.00");
	}

	private void HandlePlayeredAnted()
	{
		if (_anteButton.ButtonPressed)
		{
			_signalBus.EmitPlayerAntedSignal(this);
		}
		else
		{ 
			_signalBus.EmitPlayerAnteRemovedSignal(this);
		}
		
	}

	private bool IsPlayerAntedIn()
	{
		_isAntedIn = _anteButton.ButtonPressed;
		return _isAntedIn;
	}

	private void SetIsAntedIn(bool isAntedIn) 
	{
		_anteButton.ButtonPressed = isAntedIn;
		_isAntedIn = isAntedIn;
	}

	// TODO: Figure out a way to either unsubscribe the card to this after being delt away from player or a different way to indicate reshifting.
	// maybe using the collisiton box over the card area and not over the player icon. That way we can potentially tap into AreaExited potetntially
	// or a custom signal.
	public void HandleChildLeavingTree()
	{
		//var xCoord = Math.Clamp(DealPosition.X + (_cardInHandSpaceing * -1), 1000, 1200);
		//DealPosition = new Vector2(xCoord, DealPosition.Y);

		//DealPosition += new Vector2(_cardInHandSpaceing, 0) * -1;
	}

	public void DisableCollisionBox()
	{
		_collisionBox.SetDeferred("disabled", true);
	}

	public void EnableCollisionBox()
	{
		_collisionBox.SetDeferred("disabled", false);
	}

	public bool TryAddCard(Card card)
	{
        if (card.GetParent() != null)
		{
			card.GetParent().RemoveChild(card);
		}
		card.SetToLayFlat();
		//card.ApplyScale(new Vector2(.70f, .70f));
		card.GlobalPosition = DealPosition;

		try
		{
			_cardsInHand.CallDeferred(MethodName.AddChild, card);
            
            card.TreeExiting += HandleChildLeavingTree;
			DealPosition += new Vector2(_cardInHandSpaceing,0);
		}
		catch (Exception)
		{
			return false;
		}
		
		return true;
	}

    public void OnAreaEntered(Area2D area) 
	{
        if (area.GetGroups().Any(x => x == "card"))
        {
            var card = (Card)area;

			if (card.ModeganCardValue == 0) // If card is back card remove it
			{
				card.QueueFree();

				var signalResult = EmitSignal(SignalName.OnCardCollided, this);

				if (signalResult != Error.Ok)
				{
					GD.PrintErr(signalResult);
				}
			}
			else
			{ 
				this.TryAddCard(card);
			}
        }
    }

	public List<Card> GetCardsInHand()
	{
		var cardsInHand = _cardsInHand.GetChildren();

		if (_cardsInHand == null || !cardsInHand.Any())
		{
			return new List<Card>();
		}

		return cardsInHand.Select(x => (Card)x).ToList();
	}

	public void SetToActive()
	{ 
		_defaultPlayerImage.Visible = false;
		_activePlayerImage.Visible = true;
	}

	public void SetToPassive()
	{ 
		_defaultPlayerImage.Visible = true;
		_activePlayerImage.Visible = false;	
	}

	public void SetAnteButtonVisibility(bool shouldBeVisible)
	{ 
		_anteButton.Visible = shouldBeVisible;
	}

    public override bool Equals(object obj)
    {
		if (obj == null || !(obj is PlayerScene))
		{
			return false;
		}
		else
		{
			return this.GetInstanceId() == ((PlayerScene)obj).GetInstanceId();
		}
    }

	public void Reset()
	{
        IsAntedIn = false;
        SetAnteButtonVisibility(false);
		DealPosition = _originalGlobalMarkerPosition;//new Vector2(1000, 0);
        //_dealMarker.GlobalPosition = _defaultGlobalMarkerPosition;
	}

    private bool IsPlayerActive()
	{
		if (_activePlayerImage == null)
		{
			return false;
		}

		return _activePlayerImage.Visible;
	}
}
