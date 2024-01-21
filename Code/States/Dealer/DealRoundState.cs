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
    public partial class DealRoundState : DealerStateBase
    {
        
        private List<PlayerScene> _players;

        private SignalBus _signalBus;

        public override void _Ready()
        {
            this.State = DealerState.DealPlayerTurn;
        }

        public override async void Enter(Dictionary<string, object> parameters = null)
        {
            _players?.Clear();
            _signalBus = GetNode<SignalBus>("/root/SignalBus");

            _signalBus.EmitRequestCardBoxDisabledSignal();

            await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

        }
    }
}
