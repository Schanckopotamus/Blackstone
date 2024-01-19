using Blackstone.Code;
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

	private Vector2 _originalDealerDrawPosition;
    public Vector2 TableBoxToDealPosition { get; set; }
    public Vector2 DrawDealPosition { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_deck = CardFactory.CreateDeck();
		_cardGenerator = GetNode<CardGenerator>("CardGenerator");
		_drawMarker = GetNode<Marker2D>("DrawMarker2D");
		DrawDealPosition = _drawMarker.GlobalPosition;
		_originalDealerDrawPosition = _drawMarker.GlobalPosition;
		_dealerStateMachine = GetNode<DealerStateMachine>("StateMachine"); // DealerState.FindFirstPlayer;
		_playerOrchestrator = GetNode <PlayerOrchestrator>("/root/CardTable/PlayerOrchestrator");
		_players = _playerOrchestrator.Players;
    }
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public async void OnDealPressed()
	{
		// Deal card back card
		// Need to know what box to deal to
		var signalResult = EmitSignal(SignalName.OnDealRequested);

		if (signalResult == Error.Ok)
		{
			await Deal();
			_numCardsInfrontOfDealer = _drawMarker.GetChildren().Count;
		}
	}

    // When dealing we want the back card;
    public async Task Deal()
    {
        CheckDealerState();

		var stateParams = new Dictionary<string, object>
		{
			{ "Players", _players }
		};
		_dealerStateMachine.InitializeNewState(DealerState.FindFirstPlayer, stateParams);

		//switch (_dealerState.CurrentState.State)
		//{
		//	case DealerState.FindFirstPlayer: // When dealer pulls from card deck
		//		await DetermineFirstPlayer();
		//		break;
		//	case DealerState.DealPlayerTurn: // During the start of round for determining who is first and when Blackstones are drawn
		//		break;
		//	case DealerState.PrepNewDeck: // When dealer area is full or end of round where play continues..
		//		break;
		//	default:
		//		break;
		//}
    }

	public void RequestDeal()
	{
        EmitSignal(SignalName.OnDealRequested);
    }

 //   #region DetermineFirstPlayer
 //   private async Task DetermineFirstPlayer()
	//{
	//	PlayerScene firstPlayer = null;
	//	// Probably not the best way long term to get the number of players
		
	//	while (firstPlayer == null) 
	//	{
	//		foreach (var player in _players) 
	//		{
	//			DealToPlayer(player);

	//			GD.Print("Timer started.");
	//			await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);
	//			GD.Print("Timer ended.");
	//		}

	//		// Are the values the same?
	//		var playerOrderedGroups = _players
	//			.GroupBy(p => p.GetCardsInHand().Last().ModeganCardValue)
	//			.OrderBy(g => g.Key)
	//			.ToList();

	//		if (playerOrderedGroups.First().Count() == 1)
	//		{
	//			firstPlayer = playerOrderedGroups.First().ToList().First();
	//			//firstPlayer.SetToActive();
	//			EmitSignal(SignalName.FirstPlayerFound, firstPlayer);
 //               await DealPlayerCardsToBoxes(_players);
 //           }
	//		else
	//		{
 //               await DealPlayerCardsToBoxes(_players);
 //               _players = playerOrderedGroups.First().ToList();
	//		}
 //       }


 //       // If tie
 //       // Deal cards in players area to boxes
 //       // Start method over
 //       // else
 //       // Mark first player
 //   }

	//private void DealToPlayer(Node2D playerNode)
	//{
 //       var card = _cardGenerator.GetCard(0);
 //       //_cardGenerator.AddChild(_backCard);

 //       //var card = this.DrawCard();
	//	card.Visible = true;
	//	this.AddChild(card);
		
	//	var direction = this.GlobalPosition.DirectionTo(playerNode.GlobalPosition).Normalized();
	//	card.SetToDealt(direction, DealSpeed);

	//	//card.GlobalPosition = playerNode.GlobalPosition + new Vector2(150,0);
	//}


 //   private async Task DealPlayerCardsToBoxes(List<PlayerScene> players)
	//{
	//	foreach (var player in players)
	//	{
	//		player.CallDeferred("DisableCollisionBox");
	//	}

	//	await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);

	//	foreach (var player in players) 
	//	{
	//		var playerCards = player.GetCardsInHand();

	//		foreach (var card in playerCards) 
	//		{
	//			//card.GetParent().RemoveChild(card);

	//			if (card.ModeganCardValue != 10)
	//			{
	//				await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

	//				// Sets TableBoxToDealPosition from CardTable (Main)
	//				EmitSignal(SignalName.OnDealRequested);

	//				var direction = card.GlobalPosition.DirectionTo(TableBoxToDealPosition).Normalized();
	//				card.SetToDealt(direction, DealSpeed);
	//			}
	//		}
 //       }

 //       await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);

 //       foreach (var player in players)
 //       {
 //           player.CallDeferred("EnableCollisionBox");
 //       }
 //   }
 //   #endregion

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
