using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code.DTOs
{
    public class PlayerPopupDTO
    {
        public string PlayerName { get; private set; }
        public int NumPlayers { get; private set; }

        public PlayerPopupDTO(string playerName, int numPlayers)
        {
            PlayerName = playerName;
            NumPlayers = numPlayers;
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(PlayerName)
                && NumPlayers >= 2 && NumPlayers <= 8;
        }
    }
}
