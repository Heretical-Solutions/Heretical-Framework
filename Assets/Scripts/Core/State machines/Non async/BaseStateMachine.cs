using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.StateMachines
{
    public class BaseStateMachine<TBaseState> : IStateMachine<TBaseState>
        where TBaseState : IState
    {        
        private readonly IReadOnlyRepository<Type, TBaseState> states;
        
        private readonly IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events;
        
        private readonly Queue<ITransitionEvent<TBaseState>> transitionQueue;
        
        public BaseStateMachine(
            IReadOnlyRepository<Type, TBaseState> states,
            IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events,
            Queue<ITransitionEvent<TBaseState>> transitionQueue,
            TBaseState currentState)
        {
            this.states = states;
            
            this.events = events;
            
            this.transitionQueue = transitionQueue;
            
            CurrentState = currentState;
            
            OnCurrentStateChangeStarted = null;
            
            OnCurrentStateChangeFinished = null;
        }

        #region IStateMachine
        
        public bool TransitionInProgress { get; private set; }
        
        #region Current state
        
        public TBaseState CurrentState { get; private set; }
        
        public Action<TBaseState, TBaseState> OnCurrentStateChangeStarted { get; set; }
        
        public Action<TBaseState, TBaseState> OnCurrentStateChangeFinished { get; set; }
        
        #endregion
        
        #region All states

        public TBaseState GetState<TConcreteState>()
        {
            if (!states.TryGet(typeof(TConcreteState), out var result))
                throw new Exception($"[BaseStateMachine] STATE {typeof(TConcreteState).ToBeautifulString()} NOT FOUND");

            return result;
        }

        public TBaseState GetState(Type stateType)
        {
            if (!states.TryGet(stateType, out var result))
                throw new Exception($"[BaseStateMachine] STATE {stateType.ToBeautifulString()} NOT FOUND");

            return result;
        }

        public IEnumerable<Type> AllStates
        {
            get => states.Keys; 
        }

        #endregion
        
        #region Event handling
        
        public void Handle<TEvent>()
        {
            ITransitionEvent<TBaseState> @event;
            
            if (!events.TryGet(typeof(TEvent), out @event))
                throw new Exception($"[BaseStateMachine] EVENT {typeof(TEvent).ToBeautifulString()} NOT FOUND");
            
            if (TransitionInProgress)
                transitionQueue.Enqueue(@event);
            else
                PerformTransition(@event);
        }
        
        public void Handle(Type eventType)
        {
            ITransitionEvent<TBaseState> @event;
            
            if (!events.TryGet(eventType, out @event))
                throw new Exception($"[BaseStateMachine] EVENT {eventType.ToBeautifulString()} NOT FOUND");
            
            if (TransitionInProgress)
                transitionQueue.Enqueue(@event);
            else
                PerformTransition(@event);
        }
        
        public Action<ITransitionEvent<TBaseState>> OnEventFired { get; set; }
        
        #endregion
        
        #region Immediate transition
        
        public void TransitToImmediately<TState>()
        {
            if (!states.Has(typeof(TState)))
                throw new Exception($"[BasicStateMachine] STATE {typeof(TState).ToBeautifulString()} NOT FOUND");
            
            var previousState = CurrentState;
            
            var newState = states.Get(typeof(TState));
         
            PerformTransition(
                previousState,
                newState);
        }
        
        public void TransitToImmediately(Type stateType)
        {
            if (!states.Has(stateType))
                throw new Exception($"[BaseStateMachine] STATE {stateType.ToBeautifulString()} NOT FOUND");
            
            var previousState = CurrentState;
         
            var newState = states.Get(stateType);
         
            PerformTransition(
                previousState,
                newState);
        }
        
        #endregion
        
        #endregion
        
        private void PerformTransition(ITransitionEvent<TBaseState> @event)
        {
            //TODO: replace all default()'s with this
            if (!EqualityComparer<TBaseState>.Default.Equals(CurrentState, @event.From)) //if (CurrentState != @event.From)
            {
                string currentStateString = CurrentState.GetType().ToBeautifulString();
                
                string fromStateString = @event.From.GetType().ToBeautifulString();
                
                throw new Exception($"[BaseStateMachine] CURRENT STATE {currentStateString} IS NOT EQUAL TO TRANSITION FROM STATE {fromStateString}");
            }
            
            OnEventFired?.Invoke(@event);
            
            var previousState = CurrentState;
         
            var newState = @event.To;
            
            PerformTransition(previousState, newState);
        }
        
        private void PerformTransition(
            TBaseState previousState,
            TBaseState newState)
        {
            TransitionInProgress = true;
            
            OnCurrentStateChangeStarted?.Invoke(previousState, newState);
         
            CurrentState?.OnStateExited();
            
            CurrentState = newState;
            
            CurrentState?.OnStateEntered();
            
            OnCurrentStateChangeFinished?.Invoke(previousState, newState);
            
            TransitionInProgress = false;
            
            if (transitionQueue.Count != 0)
            {
                PerformTransition(transitionQueue.Dequeue());
            }
        }
    }
}