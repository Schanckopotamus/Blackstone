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
		_signalBus.PlayerAnteCompleted += HandleAnteCompletion;
		_signalBus.OnEndGame += HandleEndGameReset;
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

	public List<PlayerScene> GetAntedInPlayers()
	{ 
		return Players.Where(p => p.IsAntedIn).ToList();
	}

	public List<Card> GetAllPlayersCards()
	{ 
		var cards = new List<Card>();

		foreach (var p in Players) 
		{
			cards.AddRange(p.GetCardsInHand());
		}

		return cards;
	}

	public void SetCollisionBoxesOn()
	{
        foreach (var player in Players)
        {
            player.CallDeferred("DisableCollisionBox");
        }
    }

	public void SetCollisionBoxesOff()
	{
        foreach (var player in Players)
        {
            player.CallDeferred("EnableCollisionBox");
        }
    }

	private void HandleAnteCompletion()
	{ 
		var notAntedPlayers = Players.Where(p => !p.IsAntedIn).ToList();

		foreach (var player in notAntedPlayers) 
		{
			player.SetAntePositionVisibility(shouldBeVisible: false);
		}
    }

	private void ResetAnte()
	{
		foreach (var player in Players)
		{
			player.IsAntedIn = false;
			player.SetAntePositionVisibility(false);
		}
	}

	private void HandleEndGameReset()
	{
		this.SetAllToPassive();
		this.ResetAnte();
	}
}
