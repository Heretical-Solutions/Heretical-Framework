using System;

namespace HereticalSolutions.StateMachines
{
    public class TransitionRequest<TBaseState>
        where TBaseState : IState
    {
        public ITransitionEvent<TBaseState> Event { get; private set; }

        public IProgress<float> StateExitProgress { get; private set; }
        
        public IProgress<float> StateEnterProgress { get; private set; }
        
        public EAsyncTransitionState State { get; set; }

        public TransitionRequest(
            ITransitionEvent<TBaseState> @event,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null)
        {
            Event = @event;

            StateExitProgress = stateExitProgress;

            StateEnterProgress = stateEnterProgress;
            
            State = EAsyncTransitionState.QUEUED;
        }
    }
}