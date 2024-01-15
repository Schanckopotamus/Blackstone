using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code.States
{
    public class DealToSelf : IDealState
    {
        public Vector2 Target { get; set; }

        public DealToSelf(Vector2 targetDealCoordinate)
        {
            Target = targetDealCoordinate;
        }

        public void Deal()
        {
            throw new NotImplementedException();
        }
    }
}
