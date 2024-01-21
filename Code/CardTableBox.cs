using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class CardTableBox : Node2D
{
	[Signal]
	public delegate void OnCardCollidedEventHandler(CardTableBox box);

	[Signal]
	public delegate void OnCardBoxFullEventHandler(CardTableBox box);

	private HBoxContainer _container;
	private Area2D _box;
	private Rect2 _windowSize;

	private Sprite2D _md9;
	private Sprite2D _md8;
	private Sprite2D _md5;
	private Sprite2D _md4;
	private Sprite2D _md1;

	private Marker2D _m1;
	private Marker2D _m2;
	private Marker2D _m3;
	private Marker2D _m4;
	private Marker2D _m5;

	public bool IsBoxFull { get; private set; }
    public bool IsBoxActive { get; set; }
    private int _numSlots;
	private List<Marker2D> _markers = new List<Marker2D>();
	private Stack<Node2D> _slots = new Stack<Node2D>();
	private Node2D _cardStack;
	private CollisionShape2D _collisionBox;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_m1 = GetNode<Marker2D>("Box/Slot1");
		_m2 = GetNode<Marker2D>("Box/Slot2");
		_m3 = GetNode<Marker2D>("Box/Slot3");
		_m4 = GetNode<Marker2D>("Box/Slot4");
		_m5 = GetNode<Marker2D>("Box/Slot5");
		
		_box = GetNode<Area2D>("Box");
		_cardStack = GetNode<Node2D>("CardStack");
		_collisionBox = GetNode<CollisionShape2D>("Box/CollisionBox");

        IsBoxFull = false;
		IsBoxActive = false;
        _windowSize = GetViewport().GetVisibleRect();
		_markers.AddRange(new Marker2D[] {_m1, _m2, _m3, _m4, _m5});
		_numSlots = _markers.Count;

        #region Default Filled Cards
		//_md9 = GetNode<Sprite2D>("Cards/Md9");
		//_md9.GlobalPosition = _m1.GlobalPosition;

		//_md8 = GetNode<Sprite2D>("Cards/Md8");
		//_md8.GlobalPosition = _m2.GlobalPosition;

		//_md5 = GetNode<Sprite2D>("Cards/Md5");
		//_md5.GlobalPosition = _m3.GlobalPosition;

		//_md4 = GetNode<Sprite2D>("Cards/Md4");
		//_md4.GlobalPosition = _m4.GlobalPosition;

		//_md1 = GetNode<Sprite2D>("Cards/Md1");
		//_md1.GlobalPosition = _m5.GlobalPosition;
        #endregion
    }

	public bool TryAdd(Card card)
	{
		if (IsBoxFull || _slots.Count >= _numSlots)
		{
			return false;
		}
		else
		{
			// If the 'node' is already parented adding to new parent fails
			// must first be removed from a parent before adding to a new one.
			if (card.GetParent() != null)
			{
				card.GetParent().RemoveChild(card);
			}

			card.SetCollisionLayerValue(1, false);
			card.SetCollisionLayerValue(2, true);

			_cardStack.CallDeferred(Node2D.MethodName.AddChild, card);
			//_cardStack.AddChild(card);

			// Set newly added card to the corresponding marker
            var stackCount = _cardStack.GetChildren().Count;
            var markerIndex = stackCount;
			var marker = _markers[markerIndex];
			//card.ApplyScale(new Vector2(1.45f, 1.45f));
			card.GlobalScale = new Vector2(1, 1);
			card.SetToLayFlatAt(marker.Position, isGlobal: false);

			if (stackCount >= 4) // Because the child isn't added (because it is deferred) until after the call we check for 4 instead of 5 
			{
				_collisionBox.SetDeferred("disabled", true);
				IsBoxFull = true;
				IsBoxActive = false;
				EmitSignal(SignalName.OnCardBoxFull, this);
			}

			return true;
		}
	}

	public void SetCollisionDisabled(bool collisionDisabled) 
	{		
		_collisionBox.SetDeferred("disabled", collisionDisabled);
	}

	public bool GetCollisionBoxDisabled()
	{
		return _collisionBox.Disabled;
	}

	public int GetCardCount()
	{ 
		return _cardStack.GetChildren().Count;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void HandleOnAreaEntered(Area2D area)
	{
        if (area.GetGroups().Any(x => x == "card") && IsBoxActive)
        {
            var card = (Card)area;
			//card.QueueFree();

			// If card is a back card go through process of getting new card from stack
			if (card != null && card.ModeganCardValue == 0)
			{
				EmitSignal(SignalName.OnCardCollided, this);
			}
			// If card is already a revealed card then just try to add it to box.
			else if (card != null && card.ModeganCardValue > 0)
			{
				this.TryAdd(card);
			}
        }
    }
}
