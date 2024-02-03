using Blackstone.Code.Buses;
using Blackstone.Code.Enums;
using Blackstone.Code.States.Dealer;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EndRoundState : DealerStateBase
{
    private SignalBus _signalBus;

    private PlayerScene _lostPlayer;
    private List<PlayerScene> _players;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.State = DealerState.EndRound;
        _signalBus = GetNode<SignalBus>("/root/SignalBus");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void Enter(Dictionary<string, object> parameters = null)
    {
        _players =
            base.ExtractCollectionFromParameters<PlayerScene>(parameters, "Players");

        // In theory active player is the losing player unless the amount of players is 1, then is the only player left standing
        if (_players != null && _players.Any() && _players.Count() > 1)
        {
            SetLostPlayerToPrivateProperty();

            _signalBus.EmitPlayerLostSignal(_lostPlayer);
        }
        else if (_players.Count() == 1) // Only one left standing, they get the pot.
        {

        }

        // TODO: Figure out how to get remaining players winnings from the pot.
    }

    private void SetLostPlayerToPrivateProperty()
    {
        try
        {
            _lostPlayer = _players.SingleOrDefault(p => p.IsActive);
        }
        catch (Exception) // If for some reason there is more than one person active
        {
            var playersHoldingMinimumTwoBlackstones = _players.Where(p => p.GetCardsInHand().Count(pl => pl.IsBlackstone) >= 2);

            if (playersHoldingMinimumTwoBlackstones.Any()
                && playersHoldingMinimumTwoBlackstones.Count() == 1)
            {
                _lostPlayer = playersHoldingMinimumTwoBlackstones.First();
            }
            else
            {
                // Two losers, figure how to handle that.
            }
            
    }
    }

    public override void Exit()
    {
        //_dealer.Reset();
        //_activePlayer?.Dispose();
        //_players.Clear();
    }
}
