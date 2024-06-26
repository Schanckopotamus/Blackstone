using Blackstone.Code.Buses;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerOrchestrator : Node2D
{
    public List<PlayerScene> Players { get; private set; }
    public bool IsCollisionEnabled { get; set; }

    private SignalBus _signalBus;
    private CollisionOrchestrator _collisionOrchestrator;

    // This is the playerScene that represents the current players turn.
    //private PlayerScene _currentPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		IsCollisionEnabled = true;

		Players = this.GetChildren().Select(ch => (PlayerScene)ch).ToList();
		_signalBus = GetNode<SignalBus>("/root/SignalBus");
        _signalBus.PlayerFocusChanged += SetActivePlayer;
		_signalBus.PlayerAnteCompleted += HandleAnteCompletion;
		_signalBus.OnEndGame += HandleEndGameReset;
		_signalBus.EmitAnteStarted += HandleAnteStarted;
        //_signalBus.PlayerCollisionChangeRequest += HandleCollisionRequestChanged;

        _collisionOrchestrator = GetNode<CollisionOrchestrator>("/root/CollisionOrchestrator");
		_collisionOrchestrator.OnPlayerCollisionStateRequested
			+= HandleCollisionRequestChanged;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetPlayers(List<PlayerScene> players) 
	{
		Players = players;
	}

	public void ClearPlayers()
	{ 
		Players?.Clear();
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

	/// <summary>
	/// Makes sure that we set to passive and not active. Player(s) set to 
	/// active are indicated as such in the UI.
	/// </summary>
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

	private void HandleAnteCompletion()
	{ 
		var notAntedPlayers = Players.Where(p => !p.IsAntedIn).ToList();

		foreach (var player in notAntedPlayers) 
		{
			player.SetAnteButtonVisibility(shouldBeVisible: false);
		}
    }

	private void ResetPlayers()
	{
		foreach (var player in Players)
		{
			
			player.Reset();
		}
	}

	private void HandleEndGameReset()
	{
		this.SetAllToPassive();
		this.ResetPlayers();
	}

	private void HandleAnteStarted()
	{
		this.SetAllToPassive();

        foreach (var player in Players)
        {
            player.IsAntedIn = false;
            player.SetAnteButtonVisibility(true);
        }
    }

	private void HandleCollisionRequestChanged(bool isCollisionEnabled)
	{
		IsCollisionEnabled = isCollisionEnabled;

		if (isCollisionEnabled)
		{
			this.SetCollisionBoxesOn();
		}
		else
		{
			this.SetCollisionBoxesOff();
		}
	}

	public void SetCollisionBoxesOn()
	{
		foreach (var player in Players)
		{
			player.CallDeferred("EnableCollisionBox");
		}
	}

	public void SetCollisionBoxesOff()
	{
		foreach (var player in Players)
		{
			player.CallDeferred("DisableCollisionBox");
		}
	}
}
