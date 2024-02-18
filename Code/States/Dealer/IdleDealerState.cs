using Blackstone.Code.Buses;
using Blackstone.Code.DTOs;
using Blackstone.Code.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code.States.Dealer
{
    public partial class IdleDealerState : DealerStateBase
    {
        private SignalBus _signalBus;

        public override void _Ready()
        {
            this.State = DealerState.Idle;

            _signalBus = GetNode<SignalBus>("/root/SignalBus");
            _signalBus.PlayerStateChangeRequested += HandleStateChange;
        }

        private void HandleStateChange(string dealerState, Godot.Collections.Array<ParameterElement> parameters)
        {
            //EmitSignal(SignalName.DealerStateTransitionRequested, dealerState);
        }
    }
}
