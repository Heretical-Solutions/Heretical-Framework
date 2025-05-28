using System;
using System.Collections.Generic;

namespace HereticalSolutions.FSM
{
    public interface IStateMachine<TBaseState>
        where TBaseState : IState
    {
        bool TransitionInProgress { get; }
        
        #region Current state

        TBaseState CurrentState { get; }

        Action<TBaseState, TBaseState> OnCurrentStateChangeStarted { get; set; }

        Action<TBaseState, TBaseState> OnCurrentStateChangeFinished { get; set; }

        #endregion

        #region All states

        TConcreteState GetState<TConcreteState>()
            where TConcreteState : TBaseState;

        TBaseState GetState(
            Type stateType);

        IEnumerable<Type> AllStates { get; }

        #endregion
        
        #region Event handling
        
        bool Handle<TEvent>(
            bool queueIfTransitionInProgress = true,
            bool processQueueAfterFinish = true)
            where TEvent : ITransitionEvent<TBaseState>;

        bool Handle(
            Type eventType,
            bool queueIfTransitionInProgress = true,
            bool processQueueAfterFinish = true);

        Action<ITransitionEvent<TBaseState>> OnEventFired { get; }

        #endregion

        #region Immediate transition

        bool TransitToImmediately<TState>(
            bool queueIfTransitionInProgress = true,
            bool processQueueAfterFinish = true)
            where TState : TBaseState;
        
        bool TransitToImmediately(
            Type stateType,
            bool queueIfTransitionInProgress = true,
            bool processQueueAfterFinish = true);

        #endregion

        #region Scheduled transition

        IEnumerable<ITransitionRequest> ScheduledTransitions { get; }

        void ScheduleTransition(
            ITransitionRequest request,
            bool startProcessingIfIdle = true);

        void ProcessTransitionQueue();

        #endregion
    }
}