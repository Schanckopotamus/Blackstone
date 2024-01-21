using Blackstone.Code;
using Blackstone.Code.Buses;
using Blackstone.Code.DTOs;
using Blackstone.Code.Enums;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
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

	public int DealSpeed = 2500;
	private int _numCardsInfrontOfDealer = 0;
	// The number of cards the dealer will draw in front of them before sending them to the center.
	private int _dealerDrawCountMax = 7;

	private DealerStateMachine _dealerStateMachine;

	private CardDeck _deck;
	private Card _backCard;
	private CardGenerator _cardGenerator;
	private Marker2D _drawMarker;
	private PlayerOrchestrator _playerOrchestrator;
	private List<PlayerScene> _players;
	private Label _currentStatelabel;
	private SignalBus _signalBus;

	private Button _anteButton;
	private Button _dealButton;
	private Button _roundButton;

    private Vector2 _originalDealerDrawPosition;
    public Vector2 TableBoxToDealPosition { get; set; }
    public Vector2 DrawDealPosition { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _signalBus = GetNode<SignalBus>("/root/SignalBus");
		_deck = CardFactory.CreateDeck();
		_cardGenerator = GetNode<CardGenerator>("CardGenerator");
		_drawMarker = GetNode<Marker2D>("DrawMarker2D");
		DrawDealPosition = _drawMarker.GlobalPosition;
		_originalDealerDrawPosition = _drawMarker.GlobalPosition;
		_dealerStateMachine = GetNode<DealerStateMachine>("StateMachine"); // DealerState.FindFirstPlayer;
		_playerOrchestrator = GetNode<PlayerOrchestrator>("/root/CardTable/PlayerOrchestrator");
		_players = _playerOrchestrator.Players;
        _currentStatelabel = GetNode<Label>("CurrentStateContainer/Value");

        _anteButton = GetNode<Button>("Ante");
		_dealButton = GetNode<Button>("Deal");
		_roundButton = GetNode<Button>("Round");
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

    // When dealing we want the back card;
    public async Task Deal()
    {
        CheckDealerState();

		//var stateParams = new Dictionary<string, object>
		//{
		//	{ "Players", _players.Where(p => p.IsAntedIn).ToList() }
		//};
		//_dealerStateMachine.InitializeNewState(DealerState.FindFirstPlayer, stateParams);

		if (_dealerStateMachine.CurrentState.State == DealerState.PlayerAnte) 
		{
			//_dealerStateMachine.Enter();
		}
    }

	public void RequestDeal()
	{
        EmitSignal(SignalName.OnDealRequested);
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

	public void RoundReset()
	{
		//_dealerState = DealerState.DealToPlayer;
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

	private void CheckDealerState()
	{
        //if (_numCardsInfrontOfDealer >= _dealerDrawCountMax)
        //{
        //    _dealerState = DealerState.DealToBox;
        //}
        //else if (_dealerState == DealerState.DealToPlayer && _drawMarker.GetChildren().Count <= 0)
        //{
        //    _dealerState = DealerState.DealToSelf;
        //}
    }
}
