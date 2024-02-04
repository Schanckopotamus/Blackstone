using Blackstone.Code.Buses;
using Blackstone.Code.Enums;
using Blackstone.Code.States.Dealer;
using Godot;
using System;
using System.Collections.Generic;

public partial class EndGameState : DealerStateBase
{
    private SignalBus _signalBus;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.State = DealerState.EndGame;
        _signalBus = GetNode<SignalBus>("/root/SignalBus");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void Enter(Dictionary<string, object> parameters = null)
    {
        // Clean up table
        _signalBus.EmitEndGameSignal();
    }

    public override void Exit()
    {

    }
}
