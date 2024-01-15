using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code.States
{
    public interface IDealState
    {
        /// <summary>
        /// Where we want the dealer to put the card
        /// </summary>
        Vector2 Target { get; set; }
        //bool ShouldThrow { get; set; }
        //bool ShouldPlace { get; set; }

        void Deal();
    }
}
