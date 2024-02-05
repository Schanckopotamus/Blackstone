using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code.Enums
{
    public enum DealerState
    {     
        // These have specific actions for only these states
        Idle,
        PlayerAnte,
        FindFirstPlayer, // Deal cards to ante'd players to determine first player
        DealPlayerTurn, // Dealing for players turn, drawing number of specified cards and managing drawn cards
        EndRound, // End of Round, figure out money distrobution
        EndGame



        //PrepNewDeck, // Game Ends, signal for clear table and get fresh shuffled deck

        //// Common functions available for any state
        //DealToSelf,
        //DealToPlayer,
        //DealToBox
    }
}
