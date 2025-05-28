using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.NonAlloc;

using HereticalSolutions.Repositories;

using HereticalSolutions.ObjectPools.Async;

using HereticalSolutions.Logging;

namespace HereticalSolutions.FSM.NonAlloc.Async
{
    public class BaseAsyncNonAllocStateMachine<TBaseState>
        : IAsyncNonAllocStateMachine<TBaseState>
        where TBaseState : IAsyncNonAllocState
    {
        protected readonly IReadOnlyRepository<Type, TBaseState> states;

        protected readonly IReadOnlyRepository<Type, 
            IAsyncNonAllocTransitionEvent<TBaseState>> events;


        protected readonly IAsyncPool<AsyncNonAllocEventTransitionRequest>
            transitionRequestPool;

        protected readonly IAsyncPool<AsyncNonAllocImmediateTransitionRequest>
            immediateTransitionRequestPool;


        protected readonly IAsyncNonAllocTransitionController<TBaseState> 
            transitionController;

        protected readonly Queue<IAsyncNonAllocTransitionRequest> transitionQueue;


        protected readonly INonAllocSubscribable onCurrentStateChangeStarted;

        protected readonly INonAllocSubscribable onCurrentStateChangeFinished;

        protected readonly INonAllocSubscribable onEventFired;


        // Lock object for thread synchronization
        protected readonly object lockObject;

        protected readonly ILogger logger;


        protected TBaseState currentState;

        protected bool transitionInProgress;


        public BaseAsyncNonAllocStateMachine(
            IReadOnlyRepository<Type, TBaseState> states,
            IReadOnlyRepository<Type, IAsyncNonAllocTransitionEvent<TBaseState>> events,

            IAsyncPool<AsyncNonAllocEventTransitionRequest> transitionRequestPool,
            IAsyncPool<AsyncNonAllocImmediateTransitionRequest> immediateTransitionRequestPool,

            IAsyncNonAllocTransitionController<TBaseState> transitionController,
            Queue<IAsyncNonAllocTransitionRequest> transitionQueue,

            INonAllocSubscribable onCurrentStateChangeStarted,
            INonAllocSubscribable onCurrentStateChangeFinished,
            INonAllocSubscribable onEventFired,

            TBaseState initialState,

            object lockObject,

            ILogger logger)
        {
            this.states = states;

            this.events = events;


            this.transitionRequestPool = transitionRequestPool;

            this.immediateTransitionRequestPool = immediateTransitionRequestPool;


            this.transitionController = transitionController;

            this.transitionQueue = transitionQueue;


            this.onCurrentStateChangeStarted = onCurrentStateChangeStarted;

            this.onCurrentStateChangeFinished = onCurrentStateChangeFinished;

            this.onEventFired = onEventFired;
            

            this.lockObject = lockObject;


            this.logger = logger;


            currentState = initialState;

            transitionInProgress = false;
        }

        #region IAsyncStateMachine

        public bool TransitionInProgress
        {
            get
            {
                lock (lockObject)
                {
                    return transitionInProgress;
                }
            }
        }

        #region Current state

        public TBaseState CurrentState
        {
            get
            {
                lock (lockObject)
                {
                    return currentState;
                }
            }
        }

        public INonAllocSubscribable OnCurrentStateChangeStarted => 
            onCurrentStateChangeStarted;

        public INonAllocSubscribable OnCurrentStateChangeFinished => 
            onCurrentStateChangeFinished;

        #endregion

        #region All states

        public TConcreteState GetState<TConcreteState>()
            where TConcreteState : TBaseState
        {
            if (!states.TryGet(
                typeof(TConcreteState),
                out var result))
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"STATE {nameof(TConcreteState)} NOT FOUND"));
            }

            return (TConcreteState)result;
        }

        public TBaseState GetState(
            Type stateType)
        {
            if (!states.TryGet(
                stateType,
                out var result))
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"STATE {stateType.Name} NOT FOUND"));
            }

            return result;
        }

        public IEnumerable<Type> AllStates
        {
            get => states.Keys;
        }

        #endregion

        #region Transition requests

        public async Task<AsyncNonAllocEventTransitionRequest>
            PopTransitionRequest<TEvent>(

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AsyncNonAllocEventTransitionRequest transitionRequest = 
                await transitionRequestPool.Pop(
                    asyncContext);

            transitionRequest.OnPreviousStateExited.UnsubscribeAll();

            transitionRequest.OnNextStateEntered.UnsubscribeAll();

            transitionRequest.PreviousStateExitProgress = null;

            transitionRequest.NextStateEnterProgress = null;

            transitionRequest.EventType = typeof(TEvent);

            transitionRequest.AsyncContext = asyncContext;

            transitionRequest.CommencePreviousStateExitStart = true;

            transitionRequest.CommencePreviousStateExitFinish = true;

            transitionRequest.CommenceNextStateEnterStart = true;

            transitionRequest.CommenceNextStateEnterFinish = true;

            transitionRequest.TransitionState = ETransitionState.UNINITIALISED;

            return transitionRequest;
        }

        public async Task<AsyncNonAllocEventTransitionRequest>
            PopTransitionRequest(
                Type eventType,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AsyncNonAllocEventTransitionRequest transitionRequest = 
                await transitionRequestPool.Pop(asyncContext);

            transitionRequest.OnPreviousStateExited.UnsubscribeAll();

            transitionRequest.OnNextStateEntered.UnsubscribeAll();

            transitionRequest.PreviousStateExitProgress = null;

            transitionRequest.NextStateEnterProgress = null;

            transitionRequest.EventType = eventType;

            transitionRequest.AsyncContext = asyncContext;

            transitionRequest.CommencePreviousStateExitStart = true;

            transitionRequest.CommencePreviousStateExitFinish = true;

            transitionRequest.CommenceNextStateEnterStart = true;

            transitionRequest.CommenceNextStateEnterFinish = true;

            transitionRequest.TransitionState = ETransitionState.UNINITIALISED;

            return transitionRequest;
        }

        #endregion

        #region Immediate transition requests

        public async Task<AsyncNonAllocImmediateTransitionRequest>
            PopImmediateTransitionRequest<TState>(

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AsyncNonAllocImmediateTransitionRequest transitionRequest = 
                await immediateTransitionRequestPool.Pop(asyncContext);

            transitionRequest.OnPreviousStateExited.UnsubscribeAll();

            transitionRequest.OnNextStateEntered.UnsubscribeAll();

            transitionRequest.PreviousStateExitProgress = null;

            transitionRequest.NextStateEnterProgress = null;

            transitionRequest.TargetStateType = typeof(TState);

            transitionRequest.AsyncContext = asyncContext;

            transitionRequest.CommencePreviousStateExitStart = true;

            transitionRequest.CommencePreviousStateExitFinish = true;

            transitionRequest.CommenceNextStateEnterStart = true;

            transitionRequest.CommenceNextStateEnterFinish = true;

            transitionRequest.TransitionState = ETransitionState.UNINITIALISED;

            return transitionRequest;
        }

        public async Task<AsyncNonAllocImmediateTransitionRequest>
            PopImmediateTransitionRequest(
                Type stateType,

                //Async tail
                AsyncExecutionContext asyncContext)
        {
            AsyncNonAllocImmediateTransitionRequest transitionRequest = 
                await immediateTransitionRequestPool.Pop(asyncContext);

            transitionRequest.OnPreviousStateExited.UnsubscribeAll();

            transitionRequest.OnNextStateEntered.UnsubscribeAll();

            transitionRequest.PreviousStateExitProgress = null;

            transitionRequest.NextStateEnterProgress = null;

            transitionRequest.TargetStateType = stateType;

            transitionRequest.AsyncContext = asyncContext;

            transitionRequest.CommencePreviousStateExitStart = true;

            transitionRequest.CommencePreviousStateExitFinish = true;

            transitionRequest.CommenceNextStateEnterStart = true;

            transitionRequest.CommenceNextStateEnterFinish = true;

            transitionRequest.TransitionState = ETransitionState.UNINITIALISED;

            return transitionRequest;
        }

        #endregion

        public async Task RecycleTransitionRequest(
			IAsyncNonAllocTransitionRequest transitionRequest,

            //Async tail
            AsyncExecutionContext asyncContext)
		{
            transitionRequest.OnPreviousStateExited.UnsubscribeAll();

            transitionRequest.OnNextStateEntered.UnsubscribeAll();

            transitionRequest.PreviousStateExitProgress = null;

            transitionRequest.NextStateEnterProgress = null;

            transitionRequest.AsyncContext = default;

            transitionRequest.CommencePreviousStateExitStart = true;

            transitionRequest.CommencePreviousStateExitFinish = true;

            transitionRequest.CommenceNextStateEnterStart = true;

            transitionRequest.CommenceNextStateEnterFinish = true;

            transitionRequest.TransitionState = ETransitionState.UNINITIALISED;

            switch (transitionRequest)
			{
				case AsyncNonAllocEventTransitionRequest eventTransitionRequest:
				{
					eventTransitionRequest.EventType = null;

					await transitionRequestPool.Push(
						eventTransitionRequest,
                        asyncContext);

					break;
				}

				case AsyncNonAllocImmediateTransitionRequest immediateTransitionRequest:
				{
					immediateTransitionRequest.TargetStateType = null;

					await immediateTransitionRequestPool.Push(
						immediateTransitionRequest,
                        asyncContext);

					break;
				}
			}
		}

        #region Event handling

        public async Task<bool> Handle<TEvent>(

            //Async tail
            AsyncExecutionContext asyncContext,

            bool queueIfTransitionInProgress = true,
            bool processQueueAfterFinish = true)
            where TEvent : IAsyncNonAllocTransitionEvent<TBaseState>
        {
            IAsyncNonAllocTransitionEvent<TBaseState> @event;

            bool shouldQueue;
            
            lock (lockObject)
            {
                shouldQueue = transitionInProgress || transitionQueue.Count != 0;
            }

            if (shouldQueue)
            {
                if (!queueIfTransitionInProgress)
                {
                    return false;
                }

                var transitionRequest = await PopTransitionRequest<TEvent>(
                    asyncContext);

                await ScheduleTransition(
                    transitionRequest,
                    asyncContext);

                return true;
            }

            if (!events.TryGet(
                typeof(TEvent),
                out @event))
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"EVENT {nameof(TEvent)} NOT FOUND"));
            }

            await PerformTransition(
                @event,
                null,
                
                asyncContext);

            if (processQueueAfterFinish)
            {
                await ProcessTransitionQueue(
                    asyncContext);
            }

            return true;
        }

        public async Task<bool> Handle(
            Type eventType,

            //Async tail
            AsyncExecutionContext asyncContext,

            bool queueIfTransitionInProgress = true,
            bool processQueueAfterFinish = true)
        {
            IAsyncNonAllocTransitionEvent<TBaseState> @event;
            bool shouldQueue;
            
            lock (lockObject)
            {
                shouldQueue = transitionInProgress || transitionQueue.Count != 0;
            }

            if (shouldQueue)
            {
                if (!queueIfTransitionInProgress)
                {
                    return false;
                }

                var transitionRequest = await PopImmediateTransitionRequest(
                    eventType,
                    asyncContext);

                await ScheduleTransition(
                    transitionRequest,
                    asyncContext);

                return true;
            }

            if (!events.TryGet(
                eventType,
                out @event))
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"EVENT {eventType.Name} NOT FOUND"));
            }

            await PerformTransition(
                @event,
                null,
                
                asyncContext);

            if (processQueueAfterFinish)
            {
                await ProcessTransitionQueue(
                    asyncContext);
            }

            return true;
        }

        public INonAllocSubscribable OnEventFired => onEventFired;

        #endregion

        #region Immediate transition

        public async Task<bool> TransitToImmediately<TState>(

            //Async tail
            AsyncExecutionContext asyncContext,

            bool queueIfTransitionInProgress = true,
            bool processQueueAfterFinish = true)
            where TState : TBaseState
        {
            TBaseState previousState;
            TBaseState newState;
            bool shouldQueue;
            
            lock (lockObject)
            {
                shouldQueue = transitionInProgress || transitionQueue.Count != 0;
            }

            if (shouldQueue)
            {
                if (!queueIfTransitionInProgress)
                {
                    return false;
                }

                var transitionRequest = await PopImmediateTransitionRequest<TState>(
                    asyncContext);

                await ScheduleTransition(
                    transitionRequest,
                    asyncContext);

                return true;
            }

            if (!states.TryGet(
                typeof(TState),
                out newState))
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"STATE {nameof(TState)} NOT FOUND"));
            }

            lock (lockObject)
            {
                previousState = currentState;
            }
            
            await PerformTransition(
                previousState,
                newState,
                null,
                
                asyncContext);

            if (processQueueAfterFinish)
            {
                await ProcessTransitionQueue(
                    asyncContext);
            }

            return true;
        }

        public async Task<bool> TransitToImmediately(
            Type stateType,

            //Async tail
            AsyncExecutionContext asyncContext,

            bool queueIfTransitionInProgress = true,
            bool processQueueAfterFinish = true)
        {
            TBaseState previousState;
            TBaseState newState;
            bool shouldQueue;
            
            lock (lockObject)
            {
                shouldQueue = transitionInProgress || transitionQueue.Count != 0;
            }

            if (shouldQueue)
            {
                if (!queueIfTransitionInProgress)
                {
                    return false;
                }

                var transitionRequest = await PopImmediateTransitionRequest(
                    stateType,
                    asyncContext);

                await ScheduleTransition(
                    transitionRequest,
                    asyncContext);

                return true;
            }

            if (!states.TryGet(
                stateType,
                out newState))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"STATE {stateType.Name} NOT FOUND"));

            lock (lockObject)
            {
                previousState = currentState;
            }

            await PerformTransition(
                previousState,
                newState,
                null,
                
                asyncContext);

            if (processQueueAfterFinish)
            {
                await ProcessTransitionQueue(
                    asyncContext);
            }

            return true;
        }

        #endregion

        #region Scheduled transition

        public IEnumerable<IAsyncNonAllocTransitionRequest> ScheduledTransitions
        {
            get
            {
                lock (lockObject)
                {
                    // Return a copy of the queue to avoid enumeration issues
                    return new List<IAsyncNonAllocTransitionRequest>(transitionQueue);
                }
            }
        }

        public async Task ScheduleTransition(
            IAsyncNonAllocTransitionRequest request,

            //Async tail
            AsyncExecutionContext asyncContext,

            bool startProcessingIfIdle = true)
        {
            bool startProcessing;

            if (request.TransitionState != ETransitionState.UNINITIALISED)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"TRANSITION REQUEST {request.GetType().Name} ALREADY SCHEDULED"));
            }

            lock (lockObject)
            {
                transitionQueue.Enqueue(request);
                request.TransitionState = ETransitionState.QUEUED;
                startProcessing = startProcessingIfIdle && !transitionInProgress;
            }

            if (startProcessing)
            {
                await ProcessTransitionQueue(
                    asyncContext);
            }
        }

        public async Task ProcessTransitionQueue(

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool shouldProcess = false;
            
            lock (lockObject)
            {
                if (!transitionInProgress && transitionQueue.Count > 0)
                {
                    transitionInProgress = true;
                    shouldProcess = true;
                }
            }
            
            if (!shouldProcess)
            {
                return;
            }

            try
            {
                bool queueEmpty = false;

                while (!queueEmpty)
                {
                    IAsyncNonAllocTransitionRequest transitionRequest = null;
                    
                    lock (lockObject)
                    {
                        if (transitionQueue.Count > 0)
                        {
                            transitionRequest = transitionQueue.Dequeue();
                        }
                        else
                        {
                            queueEmpty = true;
                        }
                    }
                    
                    if (transitionRequest == null)
                        continue;
                    
                    switch (transitionRequest)
                    {
                        case AsyncNonAllocEventTransitionRequest eventTransitionRequest:
                        {
                            IAsyncNonAllocTransitionEvent<TBaseState> @event;

                            if (!events.TryGet(
                                eventTransitionRequest.EventType,
                                out @event))
                            {
                                throw new Exception(
                                    logger.TryFormatException(
                                        GetType(),
                                        $"EVENT {eventTransitionRequest.EventType.Name} NOT FOUND"));
                            }

                            await PerformTransition(
                                @event,
                                transitionRequest,
                                
                                asyncContext);

                            break;
                        }

                        case AsyncNonAllocImmediateTransitionRequest immediateTransitionRequest:
                        {
                            TBaseState previousState;
                            TBaseState newState;

                            if (!states.TryGet(
                                immediateTransitionRequest.TargetStateType,
                                out newState))
                            {
                                throw new Exception(
                                    logger.TryFormatException(
                                        GetType(),
                                        $"STATE {immediateTransitionRequest.TargetStateType.Name} NOT FOUND"));
                            }

                            lock (lockObject)
                            {
                                previousState = currentState;
                            }

                            await PerformTransition(
                                previousState,
                                newState,
                                transitionRequest,
                                
                                asyncContext);

                            break;
                        }
                    }
                    
                    lock (lockObject)
                    {
                        queueEmpty = transitionQueue.Count == 0;
                    }
                }
            }
            finally
            {
                lock (lockObject)
                {
                    transitionInProgress = false;
                }
            }
        }

        #endregion

        #endregion

        protected async Task PerformTransition(
            IAsyncNonAllocTransitionEvent<TBaseState> @event,
            IAsyncNonAllocTransitionRequest transitionRequest,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            TBaseState previousState;
            TBaseState newState;
            
            lock (lockObject)
            {
                if (currentState.GetType() != @event.From)
                {
                    string currentStateString = currentState.GetType().Name;
                    string transitionString = @event.GetType().Name;
                    string fromStateString = @event.From.GetType().Name;

                    throw new Exception(
                        logger.TryFormatException(
                            GetType(),
                            $"CURRENT STATE {currentStateString} IS NOT EQUAL TO TRANSITION {transitionString} PREVIOUS STATE {fromStateString}"));
                }
                
                previousState = currentState;
            }

            if (!states.TryGet(
                @event.To,
                out newState))
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"STATE {@event.To.Name} NOT FOUND"));
            }

            var publisher = onEventFired as IPublisherSingleArgGeneric<IAsyncNonAllocTransitionEvent<TBaseState>>;

            publisher?.Publish(
                @event);

            await PerformTransition(
                previousState,
                newState,
                transitionRequest,

                asyncContext);
        }

        protected async Task PerformTransition(
            TBaseState previousState,
            TBaseState newState,
            IAsyncNonAllocTransitionRequest transitionRequest,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            object[] args;

            lock (lockObject)
            {
                transitionInProgress = true;
            }

            if (transitionRequest != null)
            {
                transitionRequest.TransitionState = ETransitionState.IN_PROGRESS;
            }

            args = new object[]
            {
                previousState,
                newState
            };

            var stateChangeStartPublisher = onCurrentStateChangeStarted
                as IPublisherMultipleArgs;

            stateChangeStartPublisher?.Publish(
                args);

            // Determine the transition rule to use
            EAsyncTransitionRules transitionRule = EAsyncTransitionRules.EXIT_THEN_ENTER;
            if (transitionRequest != null)
            {
                transitionRule = transitionRequest.Rules;
            }

            switch (transitionRule)
            {
                case EAsyncTransitionRules.EXIT_THEN_ENTER:
                    // Exit previous state first, then enter the new state
                    if (transitionRequest != null)
                        await transitionController.ExitState(
                            previousState,
                            transitionRequest,
                            asyncContext);
                    else
                        await transitionController.ExitState(
                            previousState,
                            asyncContext);

                    lock (lockObject)
                    {
                        currentState = newState;
                    }

                    if (transitionRequest != null)
                        await transitionController.EnterState(
                            newState,
                            transitionRequest,
                            asyncContext);
                    else
                        await transitionController.EnterState(
                            newState,
                            asyncContext);
                    break;

                case EAsyncTransitionRules.ENTER_THEN_EXIT:
                    // Enter new state first, then exit the previous state
                    if (transitionRequest != null)
                        await transitionController.EnterState(
                            newState,
                            transitionRequest,
                            asyncContext);
                    else
                        await transitionController.EnterState(
                            newState,
                            asyncContext);

                    if (transitionRequest != null)
                        await transitionController.ExitState(
                            previousState,
                            transitionRequest,
                            asyncContext);
                    else
                        await transitionController.ExitState(
                            previousState,
                            asyncContext);

                    lock (lockObject)
                    {
                        currentState = newState;
                    }
                    break;

                case EAsyncTransitionRules.PARALLEL:
                    // Execute both exit and enter states in parallel
                    Task exitTask;
                    Task enterTask;

                    if (transitionRequest != null)
                    {
                        exitTask = transitionController.ExitState(
                            previousState,
                            transitionRequest,
                            asyncContext);

                        enterTask = transitionController.EnterState(
                            newState,
                            transitionRequest,
                            asyncContext);
                    }
                    else
                    {
                        exitTask = transitionController.ExitState(
                            previousState,
                            asyncContext);

                        enterTask = transitionController.EnterState(
                            newState,
                            asyncContext);
                    }

                    // Start both tasks and wait for them to complete
                    await Task.WhenAll(exitTask, enterTask);

                    lock (lockObject)
                    {
                        currentState = newState;
                    }
                    break;
            }

            var stateChangeFinishPublisher = onCurrentStateChangeFinished
                as IPublisherMultipleArgs;

            stateChangeFinishPublisher?.Publish(
                args);

            if (transitionRequest != null)
            {
                transitionRequest.TransitionState = ETransitionState.COMPLETED;
            }

            lock (lockObject)
            {
                transitionInProgress = false;
            }
        }
    }
}