using Blackstone.Code;
using Godot;
using System;

public partial class Dealer : Node
{
	[Signal]
	public delegate void OnDealRequestedEventHandler();

	private CardDeck _deck;
	private Card _backCard;

    public Vector2 TargetDealPosition { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_deck = CardFactory.CreateDeck();
		_backCard = CardFactory.GetCard(0);
		this.AddChild( _backCard );
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnDealPressed()
	{
		// Deal card back card
		// Need to know what box to deal to
		var error = EmitSignal(SignalName.OnDealRequested);
	}
}
