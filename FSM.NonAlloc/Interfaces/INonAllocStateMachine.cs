using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates.NonAlloc;

namespace HereticalSolutions.FSM.NonAlloc
{
    public interface INonAllocStateMachine<TBaseState>
        where TBaseState : INonAllocState
    {
        bool TransitionInProgress { get; }
        
        #region Current state

        TBaseState CurrentState { get; }

        INonAllocSubscribable OnCurrentStateChangeStarted { get; }

        INonAllocSubscribable OnCurrentStateChangeFinished { get; }

        #endregion

        #region All states

        TConcreteState GetState<TConcreteState>()
            where TConcreteState : TBaseState;

        TBaseState GetState(
            Type stateType);

        IEnumerable<Type> AllStates { get; }

        #endregion

        #region Transition requests

        INonAllocStateMachine<TBaseState> PopTransitionRequest<TEvent>(
            out NonAllocEventTransitionRequest transitionRequest);

        INonAllocStateMachine<TBaseState> PopTransitionRequest(
            Type eventType,
            out NonAllocEventTransitionRequest transitionRequest);

        #endregion

        #region Immediate transition requests

        INonAllocStateMachine<TBaseState> PopImmediateTransitionRequest<TState>(
            out NonAllocImmediateTransitionRequest transitionRequest);

        INonAllocStateMachine<TBaseState> PopImmediateTransitionRequest(
            Type stateType,
            out NonAllocImmediateTransitionRequest transitionRequest);

        #endregion

        void RecycleTransitionRequest(
            INonAllocTransitionRequest transitionRequest);

        #region Event handling

        bool Handle<TEvent>(
            bool queueIfTransitionInProgress = true,
            bool processQueueAfterFinish = true)
            where TEvent : INonAllocTransitionEvent<TBaseState>;

        bool Handle(
            Type eventType,
            bool queueIfTransitionInProgress = true,
            bool processQueueAfterFinish = true);

        INonAllocSubscribable OnEventFired { get; }

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

        IEnumerable<INonAllocTransitionRequest> ScheduledTransitions { get; }

        void ScheduleTransition(
            INonAllocTransitionRequest request,
            bool startProcessingIfIdle = true);

        void ProcessTransitionQueue();

        #endregion
    }
}