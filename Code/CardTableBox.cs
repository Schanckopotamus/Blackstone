using Godot;
using System;
using System.Collections.Generic;

public partial class CardTableBox : Node2D
{
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
	private int _numSlots;
	private List<Marker2D> _markers = new List<Marker2D>();
	private Stack<Node2D> _slots = new Stack<Node2D>();
	private Node2D _cardStack;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_m1 = GetNode<Marker2D>("Box/Slot1");
		_m2 = GetNode<Marker2D>("Box/Slot2");
		_m3 = GetNode<Marker2D>("Box/Slot3");
		_m4 = GetNode<Marker2D>("Box/Slot4");
		_m5 = GetNode<Marker2D>("Box/Slot5");

		IsBoxFull = false;
		_box = GetNode<Area2D>("Box");
		_cardStack = GetNode<Node2D>("CardStack");
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

	public bool TryAdd(Node2D node)
	{
		if (IsBoxFull || _slots.Count >= _numSlots)
		{
			return false;
		}
		else
		{
			// If the 'node' is already parented adding to new parent fails
			// must first be removed from a parent before adding to a new one.
			if (node.GetParent() != null)
			{
				node.GetParent().RemoveChild(node);
			}
			
			_cardStack.AddChild(node);

			// Set newly added card to the corresponding marker
            var stackCount = _cardStack.GetChildren().Count;
            var markerIndex = stackCount - 1;
			var marker = _markers[markerIndex];
			node.GlobalPosition = marker.GlobalPosition;

            if (stackCount == 5) 
			{
				IsBoxFull = true;
			}

			return true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void HandleOnBoxBodyEntered(Node2D node)
	{ 
		// TODO: Create custom signal in this class to notify the CardsMain script that a collision happened.
	}
}
