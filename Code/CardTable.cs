using Blackstone.Code;
using Blackstone.Code.Buses;
using Blackstone.Code.Enums;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class CardTable : Node2D
{
    private SignalBus _signalBus;
	private GameMetadata _gameMetadata;

    private PackedScene _playerDrawPackedScene;

    //private Card _card;
    private Sprite2D _testCard;
	private Label _positionLabel;
	private Label _rotationLabel;
	private Label _velocityLabel;
	private Label _potLabel;
	private Label _whitestoneCountLabel;
	private Label _anteLabel;

	private Dealer _dealer;

	private TableBoxOrchestrator _tableBoxOrchestrator;
	private PlayerOrchestrator _playerOrchestrator;
	
	// Used when players are in the game round.
	//private PlayerDrawPopup _playerDrawPopup;

    public int PotTotal 
	{
		get
		{
			return int.TryParse(_potLabel.Text, out var potAmount)
				? potAmount
				: 0;
		}
		set
		{
			_potLabel.Text = value.ToString();
		}
	}

	public int WhitestonesDealt {
		get
		{ 
			return int.TryParse(_whitestoneCountLabel.Text, out var whitestonesDealt)
				? whitestonesDealt
				: 0;
		}
		set
		{ 
			_whitestoneCountLabel.Text = value.ToString();
		}
	}

	private IndicatorLight _playerCollisionIndicator;
	private IndicatorLight _cardBoxCollisionIndicator;
	private IndicatorLight _dealerCollisionIndicator;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_signalBus = GetNode<SignalBus>("/root/SignalBus");
        _gameMetadata = GetNode<GameMetadata>("/root/GameMetadata");

        _playerDrawPackedScene = ResourceLoader.Load<PackedScene>("res://Scenes/PlayerDrawPopup.tscn");

        _signalBus.PlayerFolded += HandlePlayerFolded;
		_signalBus.WhiteStoneAdded += HandleWhiteStoneAdded;
        _signalBus.PlayerLost += HandlePlayerLost;
		_signalBus.WinningPlayersSelected += HandleWinningPlayers;
		_signalBus.OnEndGame += HandleEndGameReset;

        _dealer = GetNode<Dealer>("Dealer");
		_tableBoxOrchestrator = GetNode<TableBoxOrchestrator>("TableBoxOrchestrator");
        _playerOrchestrator = GetNode<PlayerOrchestrator>("PlayerOrchestrator");

		//_playerDrawPopup = GetNode<PlayerDrawPopup>("PlayerDrawPopup");
		//_playerDrawPopup.Hide();
        _signalBus.PlayerPopUpRequested += HandlePlayerPanelPopup;

        //TODO: Will need to be able to change the seating arrangment based on the number of players whenever a setup screen is created.
        var players = _playerOrchestrator.GetChildren();
		foreach (PlayerScene p in players)
		{
			p.OnCardCollided += OnPlayerCardCollision;
		}

		_positionLabel = GetNode<Label>("PositionContainer/CardPositionValueLabel");
		_rotationLabel = GetNode<Label>("RotationContainer/CardRotationValueLabel");
		_velocityLabel = GetNode<Label>("VelocityContainer/CardVelocityValueLabel");
		_potLabel = GetNode<Label>("PotContainer/PotValueLabel");
		_whitestoneCountLabel = GetNode<Label>("WhitestoneContainer/WhitestoneValueLabel");
		_anteLabel = GetNode<Label>("HBoxContainer/AnteLabel");
		_anteLabel.Text = _gameMetadata.GameAnte.ToString();

		_dealer.OnDealRequested += OnDealRequested;
		_dealer.FirstPlayerFound += HandleFirstPlayerFound;

		SubscribeDealerToBoxes();

		_playerCollisionIndicator = GetNode<IndicatorLight>("PlayerLight");
		_cardBoxCollisionIndicator = GetNode<IndicatorLight>("CardBoxLight");
		_dealerCollisionIndicator = GetNode<IndicatorLight>("DealerLight");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//_positionLabel.Text = _card.GlobalPosition.ToString("0.0");
		//_rotationLabel.Text = _card.GlobalRotation.ToString("0.0");
		//_velocityLabel.Text = _card.Velocity.ToString();

		// TODO: Keep track of active CardBoxPairs for visibility and active. Only one Pair should be active at any time.
		//var activePairs = _tableBoxPairs.Where(tb => tb.GetActiveBoxes().Any());
		//if (activePairs.Count() > 1)
		//{

		//}

		_playerCollisionIndicator.SetIndicator(_playerOrchestrator.IsCollisionEnabled);
		_cardBoxCollisionIndicator.SetIndicator(_tableBoxOrchestrator.IsCollisionEnabled);
		_dealerCollisionIndicator.SetIndicator(_dealer.IsCollisionEnabled);
	}

    /// <summary>
    /// This is temporary as when dealing we need to place on the table
    /// before putting them into the table boxes
    /// </summary>
    /// <returns>Returns the position of the next fillable box.</returns>
    public void OnDealRequested()
	{
		var firstFillableBox = _tableBoxOrchestrator.GetFirstBoxThatIsFillable();
		if (firstFillableBox != null) 
		{
            var boxPosition = firstFillableBox.GlobalPosition;

            if (boxPosition != _dealer.TableBoxToDealPosition)
            {
                _dealer.TableBoxToDealPosition = boxPosition;
            }
        }	
	}

    private void OnPlayerCardCollision(PlayerScene player)
    {
		var card = _dealer.DrawCard();
		player.TryAddCard(card);
    }

	private void SubscribeDealerToBoxes()
	{
        foreach (var box in _tableBoxOrchestrator.TableBoxes)
        {
			box.OnCardCollided += _dealer.DeliverCardToBox;
        }
    }

	private void HandleFirstPlayerFound(PlayerScene player)
	{
		_playerOrchestrator.SetActivePlayer(player);
	}

	private void HandlePlayerLost(PlayerScene player)
	{
		// Player pays the amount of dealt Whitestones into Pot

		var losingPenalty = WhitestonesDealt + _gameMetadata.GameAnte;

		if (player.CurrencyAmount >= losingPenalty)
		{ 
			player.CurrencyAmount -= losingPenalty;
			PotTotal += losingPenalty;

			player.SetAnteButtonVisibility(false);
		}
	}

    private void HandlePlayerPanelPopup(string name, int numPlayers)
	{
		var playerDrawScene = (PlayerDrawPopup)_playerDrawPackedScene.Instantiate();
		this.AddChild(playerDrawScene);
		playerDrawScene.Reset(name, numPlayers, 0);

		//_playerDrawPopup.Reset(name, numPlayers, 0);
		//_playerDrawPopup.Popup();
	}

	private void HandlePlayerFolded(PlayerScene player)
	{
		var ante = _gameMetadata.GameAnte;
		var foldPenalty = WhitestonesDealt != 0 ? WhitestonesDealt / 2 : 1;

		var payment = ante + foldPenalty;

		if (player.CurrencyAmount >= payment)
		{
			player.CurrencyAmount -= payment;
			PotTotal += payment;
		}
		else
		{
			PotTotal += player.CurrencyAmount;
			player.CurrencyAmount = 0;
		}

		player.SetAnteButtonVisibility(false);
    }

	private void HandleWhiteStoneAdded(Card card)
	{
		if (int.TryParse(_whitestoneCountLabel.Text, out var whiteStoneCount))
		{
			whiteStoneCount++;
			_whitestoneCountLabel.Text = whiteStoneCount.ToString();
		}
    }

	private void HandleWinningPlayers(Godot.Collections.Array<PlayerScene> winningPlayers)
	{
		var playerCount = winningPlayers.ToList().Count();

		var splitAmount = PotTotal / playerCount;		
		PotTotal = PotTotal % playerCount;// Remainder after even split

		foreach (var player in winningPlayers)
		{
			player.CurrencyAmount += splitAmount;
		}
	}

	private async void HandleEndGameReset()
	{
		// Send all cards to dealer to be dequeued
		//_signalBus.EmitRequestCardBoxDisabledSignal();
		//_playerOrchestrator.SetCollisionBoxesOff();

		WhitestonesDealt = 0;

        var cardBoxCards = _tableBoxOrchestrator.GetAllCardsInBoxes();
		var playerCards = _playerOrchestrator.GetAllPlayersCards();
		var allCards = cardBoxCards.Union(playerCards);

		foreach (var card in allCards)
		{ 
			card.QueueFree();
		}

        await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);

		_signalBus.EmitDealerStateChangeRequestedSignal(DealerState.Idle, null);
	}
}
