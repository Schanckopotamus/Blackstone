using Blackstone.Code.Enums;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code.Buses
{
    public partial class CollisionOrchestrator : Node
    {
        [Signal]
        public delegate void OnCardBoxCollisionStateRequestedEventHandler(bool isEnabled);

        [Signal]
        public delegate void OnPlayerCollisionStateRequestedEventHandler(bool isEnabled);

        [Signal]
        public delegate void OnDealerCollisionStateRequestedEventHandler(bool isEnabled);

        private DealTarget _currentTarget = DealTarget.None;

        public void ChangeDealingTarget(DealTarget target)
        {
            if (target == _currentTarget) 
            {
                return;
            }

            switch (target) 
            {
                case DealTarget.None:
                    EmitCollisionSignals(
                        enableDealerCollision: false, 
                        enablePlayerCollision: false, 
                        enableCardBoxCollision: false);
                    break;
                case DealTarget.TableBox:
                    EmitCollisionSignals(
                        enableDealerCollision: false,
                        enablePlayerCollision: false,
                        enableCardBoxCollision: true);
                    break;
                case DealTarget.Player:
                    EmitCollisionSignals(
                        enableDealerCollision: false,
                        enablePlayerCollision: true,
                        enableCardBoxCollision: false);
                    break;
                case DealTarget.Dealer:
                    EmitCollisionSignals(
                        enableDealerCollision: true,
                        enablePlayerCollision: false,
                        enableCardBoxCollision: false);
                    break;
                case DealTarget.All:
                    EmitCollisionSignals(
                        enableDealerCollision: true,
                        enablePlayerCollision: true,
                        enableCardBoxCollision: true);
                    break;
            }

            _currentTarget = target;
        }

        private void EmitCollisionSignals(bool enableDealerCollision, bool enablePlayerCollision, bool enableCardBoxCollision)
        {
            EmitDealerCollisionRequestedSignal(enableDealerCollision);
            EmitPlayerCollisionRequestedSignal(enablePlayerCollision);
            EmitTableBoxCollisionRequestedSignal(enableCardBoxCollision);
        }

        private void EmitDealerCollisionRequestedSignal(bool isEnabled)
        {
            EmitSignal(SignalName.OnDealerCollisionStateRequested, isEnabled);
        }

        private void EmitPlayerCollisionRequestedSignal(bool isEnabled)
        {
            EmitSignal(SignalName.OnPlayerCollisionStateRequested, isEnabled);
        }

        private void EmitTableBoxCollisionRequestedSignal(bool isEnabled)
        { 
            EmitSignal(SignalName.OnCardBoxCollisionStateRequested, isEnabled);
        }
    }
}
