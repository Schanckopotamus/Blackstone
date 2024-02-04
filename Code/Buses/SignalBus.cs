using Blackstone.Code.DTOs;
using Blackstone.Code.Enums;
using Blackstone.Code.Extensions;
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
        public delegate void CardBoxDisabledRequestedEventHandler();

        [Signal]
        public delegate void CardBoxEnabledRequestedEventHandler();

        [Signal]
        public delegate void PlayerFocusChangedEventHandler(PlayerScene newActivePlayer);

        [Signal]
        public delegate void PlayerStateChangeRequestedEventHandler(string newDealerStateName, Godot.Collections.Array<ParameterElement> parameters);

        [Signal]
        public delegate void PlayerAntedEventHandler(PlayerScene antedPlayer);

        [Signal]
        public delegate void PlayerAnteCompletedEventHandler();

        [Signal]
        public delegate void PlayerPopUpRequestedEventHandler(string playerName, int numPlayers);

        [Signal]
        public delegate void PlayerFoldRequestEventHandler();

        [Signal]
        public delegate void PlayerFoldedEventHandler(PlayerScene foldPlayer);

        [Signal]
        public delegate void WhiteStoneAddedEventHandler(Card card);

        [Signal]
        public delegate void DealRequestEventHandler(int numCardsToDeal);

        [Signal]
        public delegate void PlayerLostEventHandler(PlayerScene lostPlayer);

        [Signal]
        public delegate void WinningPlayersSelectedEventHandler(Godot.Collections.Array<PlayerScene> winningPlayers);

        [Signal]
        public delegate void OnEndGameEventHandler();

        public void EmitRequestCardBoxDisabledSignal()
        {
            EmitSignal(SignalName.CardBoxDisabledRequested);
        }

        public void EmitRequestCardBoxEnabledSignal() 
        {
            EmitSignal(SignalName.CardBoxEnabledRequested);
        }

        public void EmitPlayerFocusChangedSignal(PlayerScene newActivePlayer)
        {
            EmitSignal(SignalName.PlayerFocusChanged, newActivePlayer);
        }

        public void EmitPlayerStateChangeRequestedSignal(DealerState newState, Dictionary<string,object> parameters)
        {
            var godotParams = new Godot.Collections.Array<ParameterElement>();

            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    godotParams.Add(new ParameterElement(item.Key, item.Value));
                }
            }

            EmitSignal(SignalName.PlayerStateChangeRequested, newState.ToString(), godotParams);
        }

        public void EmitPlayerAntedSignal(PlayerScene antedPlayer) 
        {
            EmitSignal(SignalName.PlayerAnted, antedPlayer);
        }

        public void EmitPlayerAnteCompletedSignal()
        {
            EmitSignal(SignalName.PlayerAnteCompleted);
        }

        public void EmitPlayerPopUpRequestedSignal(PlayerPopupDTO popupInfo)
        {
            if (popupInfo != null && popupInfo.IsValid())
            {
                EmitSignal(SignalName.PlayerPopUpRequested, popupInfo.PlayerName, popupInfo.NumPlayers);
            }
            else
            {
                GD.Print("=> EmitPayerPopUpRequestedSignal NOT FIRED, PopupInfo NULL/Invalid");
            }            
        }

        public void EmitPlayerFoldRequestSignal()
        {
            EmitSignal(SignalName.PlayerFoldRequest);
        }

        public void EmitPlayerFoldedEventSignal(PlayerScene player)
        {
            EmitSignal(SignalName.PlayerFolded, player);
        }

        public void EmitWhiteStoneAddedSignal(Card card)
        {
            EmitSignal(SignalName.WhiteStoneAdded, card);
        }

        public void EmitDealRequestSignal(int numCardsToDeal)
        {
            EmitSignal(SignalName.DealRequest, numCardsToDeal);
        }

        public void EmitPlayerLostSignal(PlayerScene player)
        { 
            EmitSignal(SignalName.PlayerLost, player);
        }

        public void EmitWinningPlayersSelectedSignal(List<PlayerScene> winningPlayers)
        {
            var array = winningPlayers.ToGodotArray();

            EmitSignal(SignalName.WinningPlayersSelected, array);
        }

        public void EmitEndGameSignal()
        {
            EmitSignal(SignalName.OnEndGame);
        }
    }
}
