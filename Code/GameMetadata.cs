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
        public int GameAnte { get; private set; }
        public List<PlayerScene> Players { get; private set; }

        public override void _Ready()
        {
            GameAnte = 1;
        }

        /// <summary>
        /// Sets the ante, guarenteeing that the min is 1 and the max is 5
        /// </summary>
        /// <param name="ante"></param>
        public void SetGameAnte(int ante) 
        {
            ante = Math.Clamp(ante, 1, 5);

            GameAnte = ante;
        }

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
