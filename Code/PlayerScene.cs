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

	// The number of pixels between each card when visible on table.
	private float _cardInHandSpaceing = 35;

    private Sprite2D _defaultPlayerImage;
    private Sprite2D _activePlayerImage;
	// This is for when dealing Blackstones to player.
	private Vector2 _defaultMarkerPosition;
	private Marker2D _dealMarker;
	private Node _cardsInHand;
	private CollisionShape2D _collisionBox;

	private PlayerSeatState _seatState;

	public bool IsActive => IsPlayerActive();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_defaultPlayerImage = GetNode<Sprite2D>("DefaultPlayerImage");
		//var halvedImageSize = (_defaultPlayerImage.Texture.GetSize() / 2);
		//var shiftLeftDistance = -1 * halvedImageSize.X / 2;
		//var shiftVector = new Vector2(shiftLeftDistance, 0);

		_activePlayerImage = GetNode<Sprite2D>("ActivePlayerImage");

		_dealMarker = GetNode<Marker2D>("CardDealMarker");
		_defaultMarkerPosition = _dealMarker.GlobalPosition;
		_cardsInHand = GetNode<Node>("Cards");
		_collisionBox = GetNode<CollisionShape2D>("Box/CollisionShape2D");

		// Makes Player outline sprite active.
		this.SetToPassive();



		// to see in window when runnig scene only for testing
		//this.GlobalPosition = new Vector2(1000, 500);
		//this.Scale = new Vector2(0.15f, 0.15f);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void HandleChildLeavingTree()
	{
		_dealMarker.GlobalPosition += new Vector2(_cardInHandSpaceing, 0) * -1;
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
		card.ApplyScale(new Vector2(.70f, .70f));
		card.GlobalPosition = _dealMarker.GlobalPosition;

		try
		{
			_cardsInHand.AddChild(card);
			card.TreeExiting += HandleChildLeavingTree;
			_dealMarker.GlobalPosition += new Vector2(_cardInHandSpaceing,0);
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
			}

			var signalResult = EmitSignal(SignalName.OnCardCollided, this);

			if (signalResult != Error.Ok)
			{
				GD.PrintErr(signalResult);
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

	private bool IsPlayerActive()
	{
		if (_activePlayerImage == null)
		{
			return false;
		}

		return _activePlayerImage.Visible;
	}
}
