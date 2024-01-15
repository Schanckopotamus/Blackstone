using Blackstone.Code;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CardTable : Node2D
{
	//private Card _card;
	private Sprite2D _testCard;
	private Label _positionLabel;
	private Label _rotationLabel;
	private Label _velocityLabel;

	private Dealer _dealer;

	private TableBoxOrchestrator _tableBoxOrchestrator;
	private PlayerOrchestrator _playerOrchestrator;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_dealer = GetNode<Dealer>("Dealer");
		_tableBoxOrchestrator = GetNode<TableBoxOrchestrator>("TableBoxOrchestrator");
        _playerOrchestrator = GetNode<PlayerOrchestrator>("PlayerOrchestrator");

		//TODO: Will need to be able to change the seating arrangment based on the number of players whenever a setup screen is created.
		var players = _playerOrchestrator.GetChildren();
		foreach (PlayerScene p in players)
		{
			p.OnCardCollided += OnPlayerCardCollision;
		}

		_positionLabel = GetNode<Label>("PositionContainer/CardPositionValueLabel");
		_rotationLabel = GetNode<Label>("RotationContainer/CardRotationValueLabel");
		_velocityLabel = GetNode<Label>("VelocityContainer/CardVelocityValueLabel");
		
		_dealer.OnDealRequested += OnDealRequested;
		_dealer.FirstPlayerFound += HandleFirstPlayerFound;

		SubscribeDealerToBoxes();
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
}
