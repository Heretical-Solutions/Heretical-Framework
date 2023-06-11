using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HereticalSolutions.StateMachines
{
    public interface IAsyncStateMachine<TBaseState>
        where TBaseState : IState
    {
        bool TransitionInProgress { get; }
        
        #region Current state

        TBaseState CurrentState { get; }
        
        Action<TBaseState, TBaseState> OnCurrentStateChangeStarted { get; set; }
        
        Action<TBaseState, TBaseState> OnCurrentStateChangeFinished { get; set; }

        #endregion

        #region All states

        TBaseState GetState<TConcreteState>();

        TBaseState GetState(Type stateType);

        IEnumerable<Type> AllStates { get; }

        #endregion
        
        #region Event handling
        
        Task Handle<TEvent>(
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null);
        
        Task Handle(
            Type eventType,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null);
        
        Task Handle<TEvent>(
            CancellationToken token,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null);
        
        Task Handle(
            Type eventType,
            CancellationToken token,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null);
        
        Action<ITransitionEvent<TBaseState>> OnEventFired { get; set; }
        
        #endregion

        #region Immediate transition
        
        Task TransitToImmediately<TState>(
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null);
        
        Task TransitToImmediately(
            Type stateType,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null);
        
        Task TransitToImmediately<TState>(
            CancellationToken token,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null);
        
        Task TransitToImmediately(
            Type stateType,
            CancellationToken token,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null);
        
        #endregion
    }
}