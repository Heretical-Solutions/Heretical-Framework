using System;
using System.Threading.Tasks;

using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Delegates.NonAlloc;

namespace HereticalSolutions.FSM.NonAlloc.Async
{
    public interface IAsyncNonAllocStateMachine<TBaseState>
        where TBaseState : IAsyncNonAllocState
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

        Task<AsyncNonAllocEventTransitionRequest> PopTransitionRequest<TEvent>(
            AsyncExecutionContext asyncContext);

        Task<AsyncNonAllocEventTransitionRequest> PopTransitionRequest(
            Type eventType,
            AsyncExecutionContext asyncContext);

        #endregion

        #region Immediate transition requests

        Task<AsyncNonAllocImmediateTransitionRequest> PopImmediateTransitionRequest<TState>(
            AsyncExecutionContext asyncContext);

        Task<AsyncNonAllocImmediateTransitionRequest> PopImmediateTransitionRequest(
            Type stateType,
            AsyncExecutionContext asyncContext);

        #endregion

        Task RecycleTransitionRequest(
            IAsyncNonAllocTransitionRequest transitionRequest,

            //Async tail
            AsyncExecutionContext asyncContext);

        #region Event handling

        Task<bool> Handle<TEvent>(

            //Async tail
            AsyncExecutionContext asyncContext,

            bool queueIfTransitionInProgress = true,
            bool processQueueAfterFinish = true)
            where TEvent : IAsyncNonAllocTransitionEvent<TBaseState>;

        Task<bool> Handle(
            Type eventType,

            //Async tail
            AsyncExecutionContext asyncContext,

            bool queueIfTransitionInProgress = true,
            bool processQueueAfterFinish = true);
        
        INonAllocSubscribable OnEventFired { get; }
        
        #endregion

        #region Immediate transition
        
        Task<bool> TransitToImmediately<TState>(

            //Async tail
            AsyncExecutionContext asyncContext,

            bool queueIfTransitionInProgress = true,
            bool processQueueAfterFinish = true)
            where TState : TBaseState;
        
        Task<bool> TransitToImmediately(
            Type stateType,

            //Async tail
            AsyncExecutionContext asyncContext,

            bool queueIfTransitionInProgress = true,
            bool processQueueAfterFinish = true);

        #endregion

        #region Scheduled transition

        IEnumerable<IAsyncNonAllocTransitionRequest> ScheduledTransitions { get; }

        Task ScheduleTransition(
            IAsyncNonAllocTransitionRequest request,

            //Async tail
            AsyncExecutionContext asyncContext,

            bool startProcessingIfIdle = true);

        Task ProcessTransitionQueue(
            
            //Async tail
            AsyncExecutionContext asyncContext);

        #endregion
    }
}