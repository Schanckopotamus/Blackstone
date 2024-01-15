using Blackstone.Code.Enums;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code.States
{
    public class PlayerSeatState
    {
        public SeatPosition SeatPosition { get; set; }
        /// <summary>
        /// The number of pixels away from the center of the PlayerSeat scene.
        /// </summary>
        public Vector2 Position { get; private set; }
        /// <summary>
        /// Which side of the player icon will cards be delt
        /// </summary>
        private Vector2 DealMarkerShiftDirection { get; set; }

        public PlayerSeatState(SeatPosition seatPosition, int position = 0)
        {
            Position = Vector2.Zero;
            SeatPosition = seatPosition;

            switch (seatPosition) 
            {
                // Deal Marker Below
                case SeatPosition.East:
                case SeatPosition.West:
                    DealMarkerShiftDirection = new Vector2(0, 1);
                    break;
                // Deal to the right
                case SeatPosition.North:
                case SeatPosition.South:
                    DealMarkerShiftDirection = new Vector2(1, 0);
                    break;
                case SeatPosition.Unknown:
                    DealMarkerShiftDirection = Vector2.Zero;
                    break;
            }

            Position = DealMarkerShiftDirection * position;
        }

        /// <summary>
        /// Changes the deal Position distance
        /// </summary>
        /// <param name="distance"></param>
        public void SetPosition(int distance)
        {
            distance = Mathf.Abs(distance);

            Position = Vector2.Zero;
            Position = DealMarkerShiftDirection * distance;
        }
    }
}
