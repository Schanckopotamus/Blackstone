using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code.States.Dealer
{
    public interface IDealState
    {
        /// <summary>
        /// Where we want the dealer to put the card
        /// </summary>
        Vector2 Target { get; }
        Vector2 Dealer { get; }

        void Deal(Card card, int speed);

        void Place(Card card);
    }
}
