using Blackstone.Code.Buses;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerOrchestrator : Node2D
{
    public List<PlayerScene> Players { get; private set; }

    private SignalBus _signalBus;

    // This is the playerScene that represents the current players turn.
    //private PlayerScene _currentPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Players = this.GetChildren().Select(ch => (PlayerScene)ch).ToList();
		_signalBus = GetNode<SignalBus>("/root/SignalBus");
		_signalBus.PlayerFocusChanged += SetActivePlayer;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetActivePlayer(PlayerScene player)
	{
		var playerInstanceId = player.GetInstanceId();

		if (Players != null && Players.Any(p => p.GetInstanceId() == playerInstanceId)) 
		{
			SetAllToPassive();
			var plr = Players.First(pl => pl.GetInstanceId() == playerInstanceId);
			plr.SetToActive();
		}		
	}

	public void SetAllToPassive()
	{ 
		foreach (var plr in Players) 
		{
			plr.SetToPassive();
		}
	}

	public PlayerScene GetActivePlayer()
	{
		return Players.FirstOrDefault(p => p.IsActive);
	}
}
