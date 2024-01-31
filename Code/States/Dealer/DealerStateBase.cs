using Blackstone.Code.DTOs;
using Blackstone.Code.Enums;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code.States.Dealer
{
    public partial class DealerStateBase : Node//, IDealState
    {
        [Signal]
        public delegate void DealerStateTransitionRequestedEventHandler(string dealerStateToTransitionTo, Godot.Collections.Array<ParameterElement> parameters);

        // May not be necesary, use Signals to call the StateMachine for things like Transitions
        DealerStateMachine _stateMachine = null;

        public DealerState State { get; set; }

        public virtual void Deal(int cardsToDeal)
        { }

        public virtual void HandleInput(InputEvent inputEvent)
        { }

        public virtual void Update(float delta)
        { }

        public virtual void PhysicsUpdate(float delta) 
        { }

        /// <summary>
        /// Called by the StateMachine upon making the state active.
        /// </summary>
        /// <param name="parameters">Any parameters necessary the State Machine can hand over for the states creation</param>
        public virtual void Enter(Dictionary<string, object> parameters = null) 
        { }

        /// <summary>
        /// Use to cleanup state, like calling dispose if necessary
        /// </summary>
        public virtual void Exit()
        { }

        /// <summary>
        /// As an example, extract Players from parameter list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected List<T> ExtractCollectionFromParameters<T>(Dictionary<string, object> parameters, string key) where T : new()
        {
            var collection = new List<T>();

            if (parameters == null 
                || !parameters.Any() 
                || !parameters.ContainsKey(key))
            {
                return collection;
            }

            try
            {
                collection = (List<T>)parameters[key];

                return collection;
            }
            catch (Exception)
            {
                return collection;
            }
        }

        protected void DealCard(Card card, Vector2 targetPosition)
        { 
            
        }

        //public Vector2 Target { get; private set; }
        //public Vector2 Dealer { get; private set; }

        //public DealerStateBase(Vector2 dealerPosition, Vector2 targetPosition)
        //{
        //    Dealer = dealerPosition;
        //    Target = targetPosition;
        //}

        //public virtual void Deal(Card card, int speed)
        //{
        //    var velocity = Dealer.DirectionTo(Target).Normalized();

        //    card.SetToDealt(velocity, speed);
        //}

        //public void Place(Card card)
        //{
        //    card.SetToLayFlatAt(Target);
        //}
    }
}
