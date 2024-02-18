using Blackstone.Code;
using Blackstone.Code.Buses;
using Blackstone.Code.DTOs;
using Blackstone.Code.Enums;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

public partial class Dealer : Node2D
{
	[Signal]
	public delegate void OnDealRequestedEventHandler();

	[Signal]
	public delegate void OnCardDealtEventHandler();

	[Signal]
	public delegate void FirstPlayerFoundEventHandler(PlayerScene player);

    //[Signal]
    //public delegate void OnDealPlayerCardRequestedEventHandler();

    private SignalBus _signalBus;
    private CollisionOrchestrator _collisionOrchestrator;

    public int DealSpeed = 2500;
	private int _numCardsInfrontOfDealer = 0;
	// The number of cards the dealer will draw in front of them before sending them to the center.
	private int _dealerDrawCountMax = 7;
    // The number of pixels between each card when visible on table.
    private float _cardInHandSpaceing = 35;

    private DealerStateMachine _dealerStateMachine;

	private CardDeck _deck;
	private Card _backCard;
	private CardGenerator _cardGenerator;
	private Marker2D _drawMarker;
	private PlayerOrchestrator _playerOrchestrator;
	private Label _currentStatelabel;

	private Button _anteButton;
	private Button _dealButton;
	private Button _roundButton;

    private Vector2 _originalDealerDrawPosition;
    public Vector2 TableBoxToDealPosition { get; set; }
    public Vector2 DrawDealPosition { get; set; }

	private Label _markerPositionValue;
	private Label _markerGlobalPositionValue;

	private Node _cardsInHand;
	private CollisionShape2D _collisionBox;

    public bool IsCollisionEnabled
    {
        get
        {
            return !_collisionBox.Disabled;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _signalBus = GetNode<SignalBus>("/root/SignalBus");
		_signalBus.OnEndGame += HandleEndGameReset;

        _collisionOrchestrator = GetNode<CollisionOrchestrator>("/root/CollisionOrchestrator");
        _collisionOrchestrator.OnDealerCollisionStateRequested
            += HandleDealerCollisionChange;

        _deck = CardFactory.CreateDeck();
		_cardGenerator = GetNode<CardGenerator>("CardGenerator");

		_drawMarker = GetNode<Marker2D>("DealMarker");
		DrawDealPosition = _drawMarker.GlobalPosition;
		_originalDealerDrawPosition = _drawMarker.GlobalPosition;

		_dealerStateMachine = GetNode<DealerStateMachine>("StateMachine"); // DealerState.FindFirstPlayer;
		_playerOrchestrator = GetNode<PlayerOrchestrator>("/root/CardTable/PlayerOrchestrator");
        _currentStatelabel = GetNode<Label>("CurrentStateContainer/Value");

        _anteButton = GetNode<Button>("Ante");
		_dealButton = GetNode<Button>("Deal");
		_roundButton = GetNode<Button>("Round");

		_markerPositionValue = GetNode<Label>("DealMarker/MarkerPosition/PositionValueLabel");
		_markerPositionValue.Text = $"({_drawMarker.Position.X.ToString("0.00")},{_drawMarker.Position.Y.ToString("0.00")})";
		_markerGlobalPositionValue = GetNode<Label>("DealMarker/MarkerPosition/GPositionValueLabel");
        _markerGlobalPositionValue.Text = $"({_drawMarker.GlobalPosition.X.ToString("0.00")},{_drawMarker.GlobalPosition.Y.ToString("0.00")})";
		
		_collisionBox = GetNode<CollisionShape2D>("CollisionBox/CollisionShape2D");

        _cardsInHand = GetNode<Node>("CardsInHand");
    }
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var currentState = _dealerStateMachine.CurrentState.State;
		_currentStatelabel.Text = currentState.ToString();

		if (currentState == DealerState.Idle)
		{
			_anteButton.Disabled = false;
			_dealButton.Disabled = true;
            _roundButton.Disabled = true;
        }
		else if (currentState == DealerState.PlayerAnte)
		{
			_anteButton.Disabled = true;
			_dealButton.Disabled = false;
            _roundButton.Disabled = true;
        }
		else if (currentState == DealerState.DealPlayerTurn)
		{
            _anteButton.Disabled = true;
            _dealButton.Disabled = true;
            _roundButton.Disabled = false;
        }
		else
		{
			_anteButton.Disabled = true;
			_dealButton.Disabled = true;
            _roundButton.Disabled = true;
        }
	}

	public void SetStateLabel()
	{ 
		
	}

	public async void OnAntePressed()
	{
		//var parameterArray = new Godot.Collections.Array<ParameterElement>();

		//parameterArray.Add(new ParameterElement("Players", _players));

        _signalBus.EmitPlayerStateChangeRequestedSignal(DealerState.PlayerAnte, null);
    }

	public async void OnDealPressed()
	{
		// Deal card back card
		// Need to know what box to deal to
		var signalResult = EmitSignal(SignalName.OnDealRequested);
        

        //_dealerStateMachine

        if (signalResult == Error.Ok)
		{
            _signalBus.EmitPlayerAnteCompletedSignal();
        }
	}

	public List<Card> GetCardsInHand()
	{
		var cards = _cardsInHand?.GetChildren();

        return cards != null && cards.Any() 
			? cards.Select(c => (Card)c).ToList()
			: new List<Card>();
	}

	public void RequestDeal()
	{
        EmitSignal(SignalName.OnDealRequested);
    }

	public void DealToCardBox(Card card)
	{
        RequestDeal();

        var direction = card.GlobalPosition.DirectionTo(TableBoxToDealPosition).Normalized();
        card.SetToDealt(direction, DealSpeed, DealTarget.TableBox);
    }

    public void DeliverCardToBox(CardTableBox box)
	{ 
		var card = DrawCard();

		box.TryAdd(card);
	}

	public Card DrawCard()
	{
		var mdCard = _deck.DrawCard();
		var card = _cardGenerator.GetCard(mdCard.Value);

		card.GlobalPosition = this.GlobalPosition;

		return card;
	}

	public Card GenerateSpecificCard(int modeganValue)
	{
		return _cardGenerator.GetCard(modeganValue);
	}

	// Manage drawing here.
	// We want to reveal a max of 7 cards at time.
	public Vector2 DealToDrawPositionVector() 
	{
		// Updates draw position for the dealer. i.e. shifts the marker
		CreateNewDealerMarkerPosition();
				
		return _backCard.GlobalPosition.DirectionTo(DrawDealPosition).Normalized();

	}

	public void CardToDealer(Card card)
	{
        if (card.GetParent() != null)
        {
            card.GetParent().RemoveChild(card);
        }
        card.SetToLayFlatAt(DrawDealPosition, isGlobal: true);
		//card.ApplyScale(new Vector2(0.75f, 0.75f));

		_cardsInHand.AddChild(card);

        this.DrawDealPosition += new Vector2(_cardInHandSpaceing, 0);
    }

	public void ResetDrawPosition()
	{
        DrawDealPosition = _originalDealerDrawPosition;
    }

	private void CreateNewDealerMarkerPosition()
	{
        if (_numCardsInfrontOfDealer >= _dealerDrawCountMax)
        {
            // Move cards to boxes
            //DealMarkerCardsToCardBoxes();

            //_numCardsInfrontOfDealer = 0;
            DrawDealPosition = _originalDealerDrawPosition;
        }
		else 
		{
			var cardShiftDirection = new Vector2(1, 0) * (45*this.Scale.X);

			DrawDealPosition += cardShiftDirection;
		}        
    }

	private void HandleBodyEntered(Node2D node)
	{
		node.QueueFree();
	}

	private void HandleAreaEntered(Area2D area)
	{
		area.QueueFree();
	}

	private void HandleEndGameReset()
	{
		ResetDrawPosition();

        _deck = CardFactory.CreateDeck();

        var cardsInHand = _cardsInHand.GetChildren();
        foreach (var card in cardsInHand)
        {
            card.QueueFree();
        }
    }

	private void HandleDealerCollisionChange(bool isEnabled)
	{
		_collisionBox.SetDeferred("disabled", !isEnabled);
	}
}
