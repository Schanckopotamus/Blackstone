using Blackstone.Code;
using Blackstone.Code.Buses;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TableBoxOrchestrator : Node2D
{
    private SignalBus _signalBus;

    private CardTableBox _box1;
    private CardTableBox _box2;
    private CardTableBox _box3;
    private CardTableBox _box4;
    private CardTableBox _box5;
    private CardTableBox _box6;
    private CardTableBox _box7;
    private CardTableBox _box8;
    private CardTableBox _box9;
    private CardTableBox _box10;

    private List<CardTableBoxPair> _tableBoxPairs;
    public List<CardTableBox> TableBoxes { get; private set; }
    private bool _areBoxesDisabled;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _areBoxesDisabled = false;
        InitializeCardBoxes();

        _signalBus = GetNode<SignalBus>("/root/SignalBus");
        _signalBus.CardBoxEnabledRequested += this.HandleBoxesCollisionEnabledEvent;
        _signalBus.CardBoxDisabledRequested += this.HandleBoxesCollisionDisabledEvent;
    }

    private void HandleBoxesCollisionEnabledEvent()
    {
        _areBoxesDisabled = false;
        SetTableBoxDisabled();
    }
    
    private void HandleBoxesCollisionDisabledEvent()
    {
        _areBoxesDisabled = true;
        SetTableBoxDisabled();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void InitializeCardBoxes()
    {
        _box1 = GetNode<CardTableBox>("CardTableBox1");
        _box2 = GetNode<CardTableBox>("CardTableBox2");
        _box3 = GetNode<CardTableBox>("CardTableBox3");
        _box4 = GetNode<CardTableBox>("CardTableBox4");
        _box5 = GetNode<CardTableBox>("CardTableBox5");
        _box6 = GetNode<CardTableBox>("CardTableBox6");
        _box7 = GetNode<CardTableBox>("CardTableBox7");
        _box8 = GetNode<CardTableBox>("CardTableBox8");
        _box9 = GetNode<CardTableBox>("CardTableBox9");
        _box10 = GetNode<CardTableBox>("CardTableBox10");

        TableBoxes = new List<CardTableBox>
        {
            _box1,
            _box2,
            _box3,
            _box4,
            _box5,
            _box6,
            _box7,
            _box8,
            _box9,
            _box10
        };

        SubscribeBoxesToOnFullSignal();

        _tableBoxPairs = new List<CardTableBoxPair>
        {
            new CardTableBoxPair(_box1, _box2),
            new CardTableBoxPair(_box3, _box4),
            new CardTableBoxPair(_box5, _box6),
            new CardTableBoxPair(_box7, _box8),
            new CardTableBoxPair(_box9, _box10)
        };

        // Set the first pair active for dealing
        _tableBoxPairs[0].IsActivePair = true;

        // Initially set all other pairs but the first to not be visible. Limits the feel of a cluttered table
        _tableBoxPairs[1].SetBoxVisibility(false);
        _tableBoxPairs[2].SetBoxVisibility(false);
        _tableBoxPairs[3].SetBoxVisibility(false);
        _tableBoxPairs[4].SetBoxVisibility(false);
    }

    private void SetTableBoxDisabled()
    {
        foreach (var box in TableBoxes)
        { 
            box.SetCollisionDisabled(_areBoxesDisabled);
        }
    }

    public CardTableBox GetFirstBoxThatIsFillable()
    {
        //for (int i = 0; i < _tableBoxes.Count; i++)
        //{
        //	if (!_tableBoxes[i].IsBoxFull)
        //	{
        //		return _tableBoxes[i];
        //	}
        //}

        // Find the active pair
        // Then return the box with the fewest cards

        var activePairs = _tableBoxPairs.Where(p => p.IsActivePair);

        if (activePairs.Any())
        {
            var pair = activePairs.First();

            return pair.GetBoxWithFewerCards();
        }

        return null;
    }

    private void SubscribeBoxesToOnFullSignal()
    {
        foreach (var box in TableBoxes)
        {
            box.OnCardBoxFull += HandleOnCardBoxFull;
        }
    }

    /// <summary>
    /// Activate new pair of boxes if the last active box is full.
    /// </summary>
    private void HandleOnCardBoxFull(CardTableBox box)
    {
        var boxId = box.GetInstanceId();

        var pair = _tableBoxPairs.First(tb => tb.Box1.GetInstanceId() == boxId || tb.Box2.GetInstanceId() == boxId);

        if (!pair.IsActivePair) // Both boxes full
        {
            var pairIndex = _tableBoxPairs.IndexOf(pair);

            if (pairIndex > -1 && pairIndex < _tableBoxPairs.Count()-1)
            {
                var nextActivePair = _tableBoxPairs[++pairIndex];
                nextActivePair.SetPairAsActive();
            }
        }
    }
}