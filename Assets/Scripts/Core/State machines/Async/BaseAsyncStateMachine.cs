using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.StateMachines
{
    public class BaseAsyncStateMachine<TBaseState> : IAsyncStateMachine<TBaseState>
        where TBaseState : IState
    {
        private readonly IReadOnlyRepository<Type, TBaseState> states;

        private readonly IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events;

        private readonly Queue<TransitionRequest<TBaseState>> transitionRequestsQueue;

        private readonly ITransitionController<TBaseState> transitionController;
        
        private readonly EAsyncTransitionRules asyncTransitionRules;
        
        private CancellationTokenSource cancellationTokenSource;

        private Task currentTransition;
        
        public BaseAsyncStateMachine(
            IReadOnlyRepository<Type, TBaseState> states,
            IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events,
            Queue<TransitionRequest<TBaseState>> transitionRequestsQueue,
            ITransitionController<TBaseState> transitionController,
            EAsyncTransitionRules asyncTransitionRules,
            TBaseState currentState)
        {
            this.states = states;

            this.events = events;

            this.transitionRequestsQueue = transitionRequestsQueue;

            this.transitionController = transitionController;
            
            this.asyncTransitionRules = asyncTransitionRules;

            CurrentState = currentState;

            OnCurrentStateChangeStarted = null;

            OnCurrentStateChangeFinished = null;

            currentTransition = null;
        }

        #region IAsyncStateMachine

        public bool TransitionInProgress
        {
            get => currentTransition != null;
        }

        #region Current state
        
        public TBaseState CurrentState { get; private set; }

        public Action<TBaseState, TBaseState> OnCurrentStateChangeStarted { get; set; }

        public Action<TBaseState, TBaseState> OnCurrentStateChangeFinished { get; set; }

        #endregion
        
        #region All states

        public TBaseState GetState<TConcreteState>()
        {
            if (!states.TryGet(typeof(TConcreteState), out var result))
                throw new Exception($"[BaseAsyncStateMachine] STATE {typeof(TConcreteState).ToBeautifulString()} NOT FOUND");

            return result;
        }

        public TBaseState GetState(Type stateType)
        {
            if (!states.TryGet(stateType, out var result))
                throw new Exception($"[BaseAsyncStateMachine] STATE {stateType.ToBeautifulString()} NOT FOUND");

            return result;
        }

        public IEnumerable<Type> AllStates
        {
            get => states.Keys; 
        }

        #endregion
        
        #region Event handling
        
        public async Task Handle<TEvent>(
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null)
        {
            ITransitionEvent<TBaseState> @event;

            if (!events.TryGet(typeof(TEvent), out @event))
                throw new Exception($"[BaseAsyncStateMachine] EVENT {typeof(TEvent).ToBeautifulString()} NOT FOUND");

            if (TransitionInProgress || transitionRequestsQueue.Count != 0)
            {
                var request = new TransitionRequest<TBaseState>(
                    @event,
                    stateExitProgress,
                    stateEnterProgress);
                
                transitionRequestsQueue.Enqueue(request);

                while (request.State != EAsyncTransitionState.ABORTED
                       && request.State != EAsyncTransitionState.COMPLETED)
                {
                    if (currentTransition != null)
                        await currentTransition;
                    else
                        await Task.Yield();
                }
            }
            else
            {
                cancellationTokenSource = new CancellationTokenSource();
                
                currentTransition = PerformTransition(
                    @event,
                    cancellationTokenSource.Token,
                    null,
                    stateExitProgress,
                    stateEnterProgress);

                await currentTransition;
            }
        }

        public async Task Handle(
            Type eventType,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null)
        {
            ITransitionEvent<TBaseState> @event;

            if (!events.TryGet(eventType, out @event))
                throw new Exception($"[BaseAsyncStateMachine] EVENT {eventType.ToBeautifulString()} NOT FOUND");

            if (TransitionInProgress || transitionRequestsQueue.Count != 0)
            {
                var request = new TransitionRequest<TBaseState>(
                    @event,
                    stateExitProgress,
                    stateEnterProgress);
                
                transitionRequestsQueue.Enqueue(request);

                while (request.State != EAsyncTransitionState.ABORTED
                       && request.State != EAsyncTransitionState.COMPLETED)
                {
                    if (currentTransition != null)
                        await currentTransition;
                    else
                        await Task.Yield();
                }
            }
            else
            {
                cancellationTokenSource = new CancellationTokenSource();
                
                currentTransition = PerformTransition(
                    @event,
                    cancellationTokenSource.Token,
                    null,
                    stateExitProgress,
                    stateEnterProgress);

                await currentTransition;
            }
        }
        
        public async Task Handle<TEvent>(
            CancellationToken externalCancellationToken,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null)
        {
            ITransitionEvent<TBaseState> @event;

            if (!events.TryGet(typeof(TEvent), out @event))
                throw new Exception($"[BaseAsyncStateMachine] EVENT {typeof(TEvent).ToBeautifulString()} NOT FOUND");

            if (TransitionInProgress || transitionRequestsQueue.Count != 0)
            {
                var request = new TransitionRequest<TBaseState>(
                    @event,
                    stateExitProgress,
                    stateEnterProgress);
                
                transitionRequestsQueue.Enqueue(request);

                while (request.State != EAsyncTransitionState.ABORTED
                       && request.State != EAsyncTransitionState.COMPLETED)
                {
                    if (externalCancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (currentTransition != null)
                        await currentTransition;
                    else
                        await Task.Yield();
                }
            }
            else
            {
                cancellationTokenSource = new CancellationTokenSource();

                var cancellationToken = cancellationTokenSource.Token;

                var externalToken = externalCancellationToken;
                
                var linkedCancellationTokenSource =
                    CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, externalToken);
                
                currentTransition = PerformTransition(
                    @event,
                    linkedCancellationTokenSource.Token,
                    null,
                    stateExitProgress,
                    stateEnterProgress);

                await currentTransition;
            }
        }

        public async Task Handle(
            Type eventType,
            CancellationToken externalCancellationToken,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null)
        {
            ITransitionEvent<TBaseState> @event;

            if (!events.TryGet(eventType, out @event))
                throw new Exception($"[BaseAsyncStateMachine] EVENT {eventType.ToBeautifulString()} NOT FOUND");

            if (TransitionInProgress || transitionRequestsQueue.Count != 0)
            {
                var request = new TransitionRequest<TBaseState>(
                    @event,
                    stateExitProgress,
                    stateEnterProgress);
                
                transitionRequestsQueue.Enqueue(request);

                while (request.State != EAsyncTransitionState.ABORTED
                       && request.State != EAsyncTransitionState.COMPLETED)
                {
                    if (externalCancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (currentTransition != null)
                        await currentTransition;
                    else
                        await Task.Yield();
                }
            }
            else
            {
                cancellationTokenSource = new CancellationTokenSource();

                var cancellationToken = cancellationTokenSource.Token;

                var externalToken = externalCancellationToken;
                
                var linkedCancellationTokenSource =
                    CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, externalToken);
                
                currentTransition = PerformTransition(
                    @event,
                    linkedCancellationTokenSource.Token,
                    null,
                    stateExitProgress,
                    stateEnterProgress);

                await currentTransition;
            }
        }

        public Action<ITransitionEvent<TBaseState>> OnEventFired { get; set; }

        #endregion
        
        #region Immediate transition
        
        public async Task TransitToImmediately<TState>(
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null)
        {
            if (!states.Has(typeof(TState)))
                throw new Exception($"[BaseAsyncStateMachine] STATE {typeof(TState).ToBeautifulString()} NOT FOUND");

            var previousState = CurrentState;

            var newState = states.Get(typeof(TState));

            cancellationTokenSource?.Cancel();
            
            cancellationTokenSource?.Dispose();

            while (transitionRequestsQueue.Count > 0)
            {
                var request = transitionRequestsQueue.Dequeue();

                request.State = EAsyncTransitionState.ABORTED;
            }
            
            transitionRequestsQueue.Clear();
            
            cancellationTokenSource = new CancellationTokenSource();
            
            currentTransition = PerformTransition(
                previousState,
                newState,
                cancellationTokenSource.Token,
                null,
                stateExitProgress,
                stateEnterProgress);

            await currentTransition;
        }

        public async Task TransitToImmediately(
            Type stateType,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null)
        {
            if (!states.Has(stateType))
                throw new Exception($"[BaseAsyncStateMachine] STATE {stateType.ToBeautifulString()} NOT FOUND");

            var previousState = CurrentState;

            var newState = states.Get(stateType);

            cancellationTokenSource?.Cancel();
            
            cancellationTokenSource?.Dispose();

            while (transitionRequestsQueue.Count > 0)
            {
                var request = transitionRequestsQueue.Dequeue();

                request.State = EAsyncTransitionState.ABORTED;
            }
            
            transitionRequestsQueue.Clear();
            
            cancellationTokenSource = new CancellationTokenSource();
            
            currentTransition = PerformTransition(
                previousState,
                newState,
                cancellationTokenSource.Token,
                null,
                stateExitProgress,
                stateEnterProgress);

            await currentTransition;
        }
        
        public async Task TransitToImmediately<TState>(
            CancellationToken externalCancellationToken,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null)
        {
            if (!states.Has(typeof(TState)))
                throw new Exception($"[BaseAsyncStateMachine] STATE {typeof(TState).ToBeautifulString()} NOT FOUND");

            var previousState = CurrentState;

            var newState = states.Get(typeof(TState));

            cancellationTokenSource?.Cancel();
            
            cancellationTokenSource?.Dispose();

            while (transitionRequestsQueue.Count > 0)
            {
                var request = transitionRequestsQueue.Dequeue();

                request.State = EAsyncTransitionState.ABORTED;
            }
            
            transitionRequestsQueue.Clear();
            
            cancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = cancellationTokenSource.Token;

            var externalToken = externalCancellationToken;
                
            var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, externalToken);
                
            currentTransition = PerformTransition(
                previousState,
                newState,
                linkedCancellationTokenSource.Token,
                null,
                stateExitProgress,
                stateEnterProgress);

            await currentTransition;
        }

        public async Task TransitToImmediately(
            Type stateType,
            CancellationToken externalCancellationToken,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null)
        {
            if (!states.Has(stateType))
                throw new Exception($"[BaseAsyncStateMachine] STATE {stateType.ToBeautifulString()} NOT FOUND");

            var previousState = CurrentState;

            var newState = states.Get(stateType);

            cancellationTokenSource?.Cancel();
            
            cancellationTokenSource?.Dispose();

            while (transitionRequestsQueue.Count > 0)
            {
                var request = transitionRequestsQueue.Dequeue();

                request.State = EAsyncTransitionState.ABORTED;
            }
            
            transitionRequestsQueue.Clear();
            
            cancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = cancellationTokenSource.Token;

            var externalToken = externalCancellationToken;
                
            var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, externalToken);
                
            currentTransition = PerformTransition(
                previousState,
                newState,
                linkedCancellationTokenSource.Token,
                null,
                stateExitProgress,
                stateEnterProgress);

            await currentTransition;
        }

        #endregion
        
        #endregion
        
        private async Task PerformTransition(
            ITransitionEvent<TBaseState> @event,
            CancellationToken cancellationToken,
            TransitionRequest<TBaseState> currentRequest = null,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null)
        {
            if (!EqualityComparer<TBaseState>.Default.Equals(CurrentState, @event.From)) //(CurrentState != @event.From)
            {
                string currentStateString = CurrentState.GetType().ToBeautifulString();

                string fromStateString = @event.From.GetType().ToBeautifulString();

                throw new Exception(
                    $"[BaseAsyncStateMachine] CURRENT STATE {currentStateString} IS NOT EQUAL TO TRANSITION FROM STATE {fromStateString}");
            }

            OnEventFired?.Invoke(@event);

            var previousState = CurrentState;

            var newState = @event.To;

            await PerformTransition(
                previousState,
                newState,
                cancellationToken,
                currentRequest,
                stateExitProgress,
                stateEnterProgress);
        }

        private async Task PerformTransition(
            TBaseState previousState,
            TBaseState newState,
            CancellationToken cancellationToken,
            TransitionRequest<TBaseState> currentRequest = null,
            IProgress<float> stateExitProgress = null,
            IProgress<float> stateEnterProgress = null)
        {
            if (currentRequest != null)
                currentRequest.State = EAsyncTransitionState.IN_PROGRESS;
            
            switch (asyncTransitionRules)
            {
                case EAsyncTransitionRules.PERFORM_TRANSITION_SEQUENTIALLY:
                    
                    OnCurrentStateChangeStarted?.Invoke(
                        previousState,
                        newState);

                    await transitionController.ExitState(
                        previousState,
                        cancellationToken,
                        stateExitProgress);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        if (currentRequest != null)
                            currentRequest.State = EAsyncTransitionState.ABORTED;
                        
                        return;
                    }

                    CurrentState?.OnStateExited();

                    CurrentState = newState;

                    await transitionController.EnterState(
                        newState,
                        cancellationToken,
                        stateEnterProgress);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        if (currentRequest != null)
                            currentRequest.State = EAsyncTransitionState.ABORTED;
                        
                        return;
                    }

                    CurrentState?.OnStateEntered();

                    OnCurrentStateChangeFinished?.Invoke(previousState, newState);
                    
                    break;

                case EAsyncTransitionRules.PERFORM_TRANSITION_SIMULTANEOUSLY:

                    OnCurrentStateChangeStarted?.Invoke(previousState, newState);

                    await Task.WhenAll(
                        transitionController
                            .ExitState(
                                previousState,
                                cancellationToken,
                                stateExitProgress)
                            .ContinueWith((task) =>
                            {
                                previousState?.OnStateExited();
                            }),
                        transitionController
                            .EnterState(
                                newState,
                                cancellationToken,
                                stateEnterProgress)
                            .ContinueWith((task) =>
                            {
                                newState?.OnStateEntered();
                            }));

                    if (cancellationToken.IsCancellationRequested)
                    {
                        if (currentRequest != null)
                            currentRequest.State = EAsyncTransitionState.ABORTED;
                        
                        return;
                    }
                    
                    CurrentState = newState;

                    previousState?.OnStateExited();

                    newState?.OnStateEntered();

                    OnCurrentStateChangeFinished?.Invoke(previousState, newState);

                    break;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                if (currentRequest != null)
                    currentRequest.State = EAsyncTransitionState.ABORTED;
                
                return;
            }
            
            if (currentRequest != null)
                currentRequest.State = EAsyncTransitionState.COMPLETED;
            
            if (transitionRequestsQueue.Count != 0)
            {
                var nextRequest = transitionRequestsQueue.Dequeue();

                currentTransition = PerformTransition(
                    nextRequest.Event,
                    cancellationToken,
                    nextRequest,
                    nextRequest.StateExitProgress,
                    nextRequest.StateEnterProgress);
            }
            else
            {
                currentTransition = null;
                
                cancellationTokenSource.Dispose();
            }
        }
    }
}