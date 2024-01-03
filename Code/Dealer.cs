using Blackstone.Code;
using Godot;
using System;

public partial class Dealer : Node2D
{
	[Signal]
	public delegate void OnDealRequestedEventHandler();

	[Signal]
	public delegate void OnCardDealtEventHandler();

	private int _dealSpeed = 1600;

	private CardDeck _deck;
	private Card _backCard;
	private CardGenerator _cardGenerator;

    public Vector2 TargetDealPosition { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_deck = CardFactory.CreateDeck();
		_cardGenerator = GetNode<CardGenerator>("CardGenerator");
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnDealPressed()
	{
		// Deal card back card
		// Need to know what box to deal to
		var signalResult = EmitSignal(SignalName.OnDealRequested);

		if (signalResult == Error.Ok)
		{
			Deal();
		}
	}

	public void DeliverCardToBox(CardTableBox box)
	{ 
		var card = NextCard();

		box.TryAdd(card);
	}

	public Card NextCard()
	{
		var mdCard = _deck.DrawCard();
		var card = _cardGenerator.GetCard(mdCard.Value);

		return card;
	}

	// When dealing we want the back card;
	public void Deal()
	{
		_backCard = _cardGenerator.GetCard(0);
		_cardGenerator.AddChild(_backCard);

		_backCard.Position = this.GlobalPosition;
		_backCard.Visible = true;
		var vectorToTarget = _backCard.GlobalPosition.DirectionTo(TargetDealPosition).Normalized();
		_backCard.Velocity = vectorToTarget;
		_backCard.Speed = _dealSpeed;
		_backCard.isDealt = true;
	}
}
