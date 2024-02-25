using Blackstone.Code.Buses;
using Blackstone.Code.Enums;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code.States.Dealer
{
    public partial class AntePlayerState : DealerStateBase
    {
        private SignalBus _signalBus;
        private List<PlayerScene> _players;

        public override void _Ready()
        {
            this.State = DealerState.PlayerAnte;
            _signalBus = GetNode<SignalBus>("/root/SignalBus");
            _signalBus.PlayerAnted += HandlePlayerAnted;
            _signalBus.PlayerAnteCompleted += HandlePlayerAntedCompleted;
            _signalBus.PlayerAnteRemoved += HandlePlayerRemovedAnte;
            _players = new List<PlayerScene>();
        }

        public override void Enter(Dictionary<string, object> parameters = null)
        {
            _signalBus.EmitAnteStartedSignal();

            if (_players.Any())
            {
                _players.Clear();
            }            
        }

        public override void Exit()
        {
            //_players.Clear();

            // This may need to be sent out on whatever the trigger condition will be when Anteing is done outside of this state
            // This state would then listen to this signal and then trigger state change.
            //_signalBus.EmitPlayerAnteCompletedSignal();
        }

        private void HandlePlayerAnted(PlayerScene player) 
        {
            if (!_players.Contains(player))
            {
                _players.Add(player);
            }
        }

        private void HandlePlayerRemovedAnte(PlayerScene player) 
        {
            _players.Remove(player);
        }

        private void HandlePlayerAntedCompleted()
        {
            var parameters = new Dictionary<string, object>()
            {
                { "Players", _players }
            };
            _signalBus.EmitDealerStateChangeRequestedSignal(DealerState.FindFirstPlayer, parameters);
        }
    }
}
