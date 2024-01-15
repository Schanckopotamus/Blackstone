using Godot;
using System;

public partial class GameOrchestrator : Node2D
{
    public GameState GameState { get; set; }
	public Dealer Dealer { get; private set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		GameState = GameState.GameSetup;
		Dealer = GetNode<Dealer>("../Dealer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//switch (GameState)
		//{
		//	case GameState.GameSetup:
		//		break;
		//	case GameState.StartGame:
		//		break;
		//	case GameState.StartRound:
		//		break;
		//	case GameState.GameRound:
		//		break;
		//	case GameState.EndRound:
		//		break;
		//	case GameState.EndGame:
		//		break;
		//	default:
		//		break;
		//}
	}
}

public enum GameState
{ 
	GameSetup, // Load Screen or subscreen that asks things like number of players
	StartGame, // Anteing
	StartRound, // Figuring out who is first
	GameRound, // Player asking for number of cards dealing, folding
	EndRound, // Winner(s) determined, pot split amounst survivors
	EndGame // Player leaves game/table
}
