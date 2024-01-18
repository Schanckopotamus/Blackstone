using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code.Buses
{
    /// <summary>
    /// Autoloaded object that stores signals that are intended to be global.
    /// Should be used for signals that are more high level and not specific
    /// to any one scene.
    /// </summary>
    public partial class SignalBus : Node
    {
        [Signal]
        public delegate void RequestCardBoxDisabledEventHandler();

        [Signal]
        public delegate void RequestCardBoxEnabledEventHandler();

        [Signal]
        public delegate void PlayerFocusChangedEventHandler(PlayerScene newActivePlayer);

        public void EmitRequestCardBoxDisabledSignal()
        {
            EmitSignal(SignalName.RequestCardBoxDisabled);
        }

        public void EmitRequestCardBoxEnabledSignal() 
        {
            EmitSignal(SignalName.RequestCardBoxEnabled);
        }

        public void EmitPlayerFocusChangedSignal(PlayerScene newActivePlayer)
        {
            EmitSignal(SignalName.PlayerFocusChanged, newActivePlayer);
        }
    }
}
