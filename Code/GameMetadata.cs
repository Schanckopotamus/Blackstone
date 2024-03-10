using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code
{
    /// <summary>
    /// Hold state for data that neads to persist through menus
    /// </summary>
    public partial class GameMetadata : Node
    {
        public List<PlayerScene> Players { get; private set; }

        public void AddPlayer(PlayerScene player)
        { 
            this.Players.Add(player); 
        }

        public void AddPlayers(List<PlayerScene> players)
        { 
            Players.AddRange(players);
        }

        public void SetPlayers(List<PlayerScene> players) 
        {
            Players = players;
        }

        public void RemovePlayer(PlayerScene player) 
        {
            if (this.Players.Contains(player)) 
            {
                Players.Remove(player);
            }
        }

        public void ClearPlayers()
        { 
            Players.Clear(); 
        }
    }
}
