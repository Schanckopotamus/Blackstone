using Blackstone.Code.Enums;
using Blackstone.Code.States.Dealer;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DealerStateMachine : DealerStateBase
{
	// Signal for Dealer to notify when state changed
	[Signal]
	public delegate void OnStateTransitionedEventHandler(DealerState newDealerState);

	[Export]
	public string InitialStatePath { get; set; }

	public DealerStateBase CurrentState { get; internal set; }

	/// <summary>
	/// Holds all possible dealer states
	/// </summary>
	private List<DealerStateBase> _states;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_states = this.GetChildren().Select(ch => (DealerStateBase)ch).ToList();

		CurrentState = GetNode<DealerStateBase>(InitialStatePath);
		if (CurrentState != null) 
		{
			CurrentState.DealerStateTransitionRequested += this.ConnectToDealerStateSignal;
			CurrentState.Enter();
		}
		

		//InitialStateFindFirstPlayer();
	}

	private void InitialStateFindFirstPlayer()
	{
        CurrentState = GetNode<DealerStateBase>(InitialStatePath);
        CurrentState.DealerStateTransitionRequested += this.ConnectToDealerStateSignal;

		var paramDict = new Dictionary<string, object>
		{
			{ "DealerState", DealerState.FindFirstPlayer },
			{ "Players", new List<PlayerScene>() }
		};

        CurrentState.Enter(paramDict);
    }

	public void UnhandledInput(InputEvent inputEvent)
	{ 
		CurrentState.HandleInput(inputEvent);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		CurrentState?.Update((float)delta);
	}

    public override void _PhysicsProcess(double delta)
    {
        CurrentState?.PhysicsUpdate((float)delta);
    }

	public void InitializeNewState(DealerState dealerState, Dictionary<string, object> parameters)
	{
        var newState = _states.FirstOrDefault(s => s.State == dealerState);

		if (!parameters.ContainsKey("DealerState"))
		{
			parameters.Add("DealerState", dealerState);
		}

        if (newState != null)
        {
            this.CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter(parameters);
            EmitSignal(SignalName.OnStateTransitioned, CurrentState);
        }
    }

    private void ConnectToDealerStateSignal(string dealerState)
	{
		if (Enum.TryParse<DealerState>(dealerState, out var dState))
		{
			var newState = _states.FirstOrDefault(s => s.State == dState);

			if (newState != null)
			{
				this.CurrentState.Exit();
				CurrentState = newState;
				CurrentState.Enter(new Dictionary<string, object> { { "DealerState", dState } });
				EmitSignal(SignalName.OnStateTransitioned, CurrentState);
			}
		}
		else
		{ 
			// SOMETHING WENT TERRIBLY WRONG
		}
	}
}
