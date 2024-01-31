using Blackstone.Code.Buses;
using Blackstone.Code.DTOs;
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
    private SignalBus _signalBus;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _signalBus = GetNode<SignalBus>("/root/SignalBus");

		InitializeStates();
        
		CurrentState = _states.FirstOrDefault(st => st.State == DealerState.Idle);


		CurrentState = GetNode<DealerStateBase>("Idle");
		if (CurrentState != null) 
		{
			_signalBus.PlayerStateChangeRequested += this.ConnectToDealerStateSignal;
			CurrentState.Enter();
		}
	}

	private void InitializeStates()
	{
        _states = this.GetChildren().Select(ch => (DealerStateBase)ch).ToList();

        foreach (var state in _states) 
		{
            _signalBus.DealRequest += state.Deal;
        }
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

	//public void InitializeNewState(DealerState dealerState, Godot.Collections.Array<ParameterElement> parameters)
	//{
	//	var paramDict = parameters.ToParamDictionary();

	//       var newState = _states.FirstOrDefault(s => s.State == dealerState);

	//	if (!paramDict.ContainsKey("DealerState"))
	//	{
	//           paramDict.Add("DealerState", dealerState.ToString());
	//	}

	//       if (newState != null)
	//       {
	//           this.CurrentState?.Exit();
	//           CurrentState = newState;
	//           CurrentState.Enter(paramDict);
	//           EmitSignal(SignalName.OnStateTransitioned, CurrentState);
	//       }
	//   }

	//public void TriggerExitState()
	//{
	//	CurrentState.HandleInput(null);
	//}

    private void ConnectToDealerStateSignal(string dealerState, Godot.Collections.Array<ParameterElement> parameters)
	{
        var paramDict = parameters.ToParamDictionary();

        if (Enum.TryParse<DealerState>(dealerState, out var dState))
		{
			var newState = _states.FirstOrDefault(s => s.State == dState);

			if (newState != null)
			{
				CurrentState.Exit();
				CurrentState = newState;

                if (!paramDict.ContainsKey("DealerState"))
				{
                    paramDict.Add("DealerState", CurrentState);
				}

				CurrentState.Enter(paramDict);
				EmitSignal(SignalName.OnStateTransitioned, CurrentState);
			}
		}
		else
		{ 
			// SOMETHING WENT TERRIBLY WRONG
		}
	}
}
