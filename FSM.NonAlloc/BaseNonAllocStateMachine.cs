using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.NonAlloc;

using HereticalSolutions.Repositories;

using HereticalSolutions.ObjectPools;

using HereticalSolutions.Logging;

namespace HereticalSolutions.FSM.NonAlloc
{
	public class BaseNonAllocStateMachine<TBaseState>
		: INonAllocStateMachine<TBaseState>
		where TBaseState : INonAllocState
	{
		protected readonly IReadOnlyRepository<Type, TBaseState> states;

		protected readonly IReadOnlyRepository<
			Type,
			INonAllocTransitionEvent<TBaseState>>
			events;

		protected readonly IPool<NonAllocEventTransitionRequest> 
			transitionRequestPool;

		protected readonly IPool<NonAllocImmediateTransitionRequest>
			immediateTransitionRequestPool;

		protected readonly INonAllocTransitionController<TBaseState> transitionController;

		protected readonly Queue<INonAllocTransitionRequest> transitionQueue;


		protected readonly INonAllocSubscribable onCurrentStateChangeStarted;

		protected readonly INonAllocSubscribable onCurrentStateChangeFinished;

		protected readonly INonAllocSubscribable onEventFired;


		protected readonly ILogger logger;


		protected bool transitionInProgress;

		protected TBaseState currentState;

		public BaseNonAllocStateMachine(
			IReadOnlyRepository<Type, TBaseState> states,
			IReadOnlyRepository<Type, INonAllocTransitionEvent<TBaseState>> events,

			IPool<NonAllocEventTransitionRequest> transitionRequestPool,
			IPool<NonAllocImmediateTransitionRequest> immediateTransitionRequestPool,

			INonAllocTransitionController<TBaseState> transitionController,
			Queue<INonAllocTransitionRequest> transitionQueue,

			INonAllocSubscribable onCurrentStateChangeStarted,
			INonAllocSubscribable onCurrentStateChangeFinished,
			INonAllocSubscribable onEventFired,

			TBaseState initialState,

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


			this.logger = logger;


			currentState = initialState;

			transitionInProgress = false;
		}

		#region IStateMachine

		public bool TransitionInProgress => transitionInProgress;

		#region Current state

		public TBaseState CurrentState => currentState;

		public INonAllocSubscribable OnCurrentStateChangeStarted => onCurrentStateChangeStarted;

		public INonAllocSubscribable OnCurrentStateChangeFinished => onCurrentStateChangeFinished;

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

		public INonAllocStateMachine<TBaseState> PopTransitionRequest<TEvent>(
			out NonAllocEventTransitionRequest transitionRequest)
		{
			transitionRequest = transitionRequestPool.Pop();

			transitionRequest.OnPreviousStateExited.UnsubscribeAll();

			transitionRequest.OnNextStateEntered.UnsubscribeAll();
			
			transitionRequest.PreviousStateExitProgress = null;

			transitionRequest.NextStateEnterProgress = null;

			transitionRequest.EventType = typeof(TEvent);

			transitionRequest.TransitionState = ETransitionState.UNINITIALISED;

			return this;
		}

		public INonAllocStateMachine<TBaseState> PopTransitionRequest(
			Type eventType,
			out NonAllocEventTransitionRequest transitionRequest)
		{
			transitionRequest = transitionRequestPool.Pop();

			transitionRequest.OnPreviousStateExited.UnsubscribeAll();

			transitionRequest.OnNextStateEntered.UnsubscribeAll();

			transitionRequest.PreviousStateExitProgress = null;

			transitionRequest.NextStateEnterProgress = null;

			transitionRequest.EventType = eventType;

			transitionRequest.TransitionState = ETransitionState.UNINITIALISED;

			return this;
		}

		#endregion

		#region Immediate transition requests

		public INonAllocStateMachine<TBaseState> PopImmediateTransitionRequest<TState>(
			out NonAllocImmediateTransitionRequest transitionRequest)
		{
			transitionRequest = immediateTransitionRequestPool.Pop();

			transitionRequest.OnPreviousStateExited.UnsubscribeAll();

			transitionRequest.OnNextStateEntered.UnsubscribeAll();

			transitionRequest.PreviousStateExitProgress = null;

			transitionRequest.NextStateEnterProgress = null;

			transitionRequest.TargetStateType = typeof(TState);

			transitionRequest.TransitionState = ETransitionState.UNINITIALISED;

			return this;
		}

		public INonAllocStateMachine<TBaseState> PopImmediateTransitionRequest(
			Type stateType,
			out NonAllocImmediateTransitionRequest transitionRequest)
		{
			transitionRequest = immediateTransitionRequestPool.Pop();

			transitionRequest.OnPreviousStateExited.UnsubscribeAll();

			transitionRequest.OnNextStateEntered.UnsubscribeAll();

			transitionRequest.PreviousStateExitProgress = null;

			transitionRequest.NextStateEnterProgress = null;

			transitionRequest.TargetStateType = stateType;

			transitionRequest.TransitionState = ETransitionState.UNINITIALISED;

			return this;
		}

		#endregion

		public void RecycleTransitionRequest(
			INonAllocTransitionRequest transitionRequest)
		{
			transitionRequest.OnPreviousStateExited.UnsubscribeAll();

			transitionRequest.OnNextStateEntered.UnsubscribeAll();

			transitionRequest.PreviousStateExitProgress = null;

			transitionRequest.NextStateEnterProgress = null;

			transitionRequest.TransitionState = ETransitionState.UNINITIALISED;

			switch (transitionRequest)
			{
				case NonAllocEventTransitionRequest eventTransitionRequest:
				{
					eventTransitionRequest.EventType = null;

					transitionRequestPool.Push(
						eventTransitionRequest);

					break;
				}

				case NonAllocImmediateTransitionRequest immediateTransitionRequest:
				{
					immediateTransitionRequest.TargetStateType = null;

					immediateTransitionRequestPool.Push(
						immediateTransitionRequest);

					break;
				}
			}
		}

		#region Event handling

		public bool Handle<TEvent>(
			bool queueIfTransitionInProgress = true,
			bool processQueueAfterFinish = true)
			where TEvent : INonAllocTransitionEvent<TBaseState>
		{
			if (transitionInProgress
				|| transitionQueue.Count != 0)
			{
				if (!queueIfTransitionInProgress)
				{
					return false;
				}

				PopTransitionRequest<TEvent>(
					out var transitionRequest);

				ScheduleTransition(
					transitionRequest);

				return true;
			}

			INonAllocTransitionEvent<TBaseState> @event;

			if (!events.TryGet(
				typeof(TEvent),
				out @event))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"EVENT {nameof(TEvent)} NOT FOUND"));
			}

			PerformTransition(
				@event,
				null);

			if (processQueueAfterFinish)
			{
				ProcessTransitionQueue();
			}

			return true;
		}

		public bool Handle(
			Type eventType,
			bool queueIfTransitionInProgress = true,
			bool processQueueAfterFinish = true)
		{
			if (transitionInProgress
				|| transitionQueue.Count != 0)
			{
				if (!queueIfTransitionInProgress)
				{
					return false;
				}

				PopTransitionRequest(
					eventType,
					out var transitionRequest);

				ScheduleTransition(
					transitionRequest);

				return true;
			}

			INonAllocTransitionEvent<TBaseState> @event;

			if (!events.TryGet(
				eventType,
				out @event))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"EVENT {eventType.Name} NOT FOUND"));
			}

			PerformTransition(
				@event,
				null);

			if (processQueueAfterFinish)
			{
				ProcessTransitionQueue();
			}

			return true;
		}

		public INonAllocSubscribable OnEventFired => onEventFired;

		#endregion

		#region Immediate transition

		public bool TransitToImmediately<TState>(
			bool queueIfTransitionInProgress = true,
			bool processQueueAfterFinish = true)
			where TState : TBaseState
		{
			if (transitionInProgress
				|| transitionQueue.Count != 0)
			{
				if (!queueIfTransitionInProgress)
				{
					return false;
				}

				PopImmediateTransitionRequest<TState>(
					out var transitionRequest);

				ScheduleTransition(
					transitionRequest);

				return true;
			}

			if (!states.TryGet(
				typeof(TState),
				out var newState))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"STATE {nameof(TState)} NOT FOUND"));
			}

			var previousState = currentState;

			PerformTransition(
				previousState,
				newState,
				null);

			if (processQueueAfterFinish)
			{
				ProcessTransitionQueue();
			}

			return true;
		}

		public bool TransitToImmediately(
			Type stateType,
			bool queueIfTransitionInProgress = true,
			bool processQueueAfterFinish = true)
		{
			if (transitionInProgress
				|| transitionQueue.Count != 0)
			{
				if (!queueIfTransitionInProgress)
				{
					return false;
				}

				PopImmediateTransitionRequest(
					stateType,
					out var transitionRequest);

				ScheduleTransition(
					transitionRequest);

				return true;
			}

			if (!states.TryGet(
				stateType,
				out var newState))
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"STATE {stateType.Name} NOT FOUND"));

			var previousState = currentState;

			PerformTransition(
				previousState,
				newState,
				null);

			if (processQueueAfterFinish)
			{
				ProcessTransitionQueue();
			}

			return true;
		}

		#endregion
	
		#region Scheduled transition
	
		public IEnumerable<INonAllocTransitionRequest> ScheduledTransitions => 
			transitionQueue;
	
		public void ScheduleTransition(
			INonAllocTransitionRequest request,
			bool startProcessingIfIdle = true)
		{
			if (request.TransitionState != ETransitionState.UNINITIALISED)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"TRANSITION REQUEST {request.GetType().Name} ALREADY SCHEDULED"));
			}

			transitionQueue.Enqueue(request);

			request.TransitionState = ETransitionState.QUEUED;

			if (startProcessingIfIdle
				&& !transitionInProgress)
			{
				ProcessTransitionQueue();
			}
		}

		public void ProcessTransitionQueue()
		{
			if (transitionInProgress)
			{
				return;
			}

			if (transitionQueue.Count == 0)
			{
				return;
			}

			while (transitionQueue.Count != 0)
			{
				var transitionRequest = transitionQueue.Dequeue();

				switch (transitionRequest)
				{
					case NonAllocEventTransitionRequest eventTransitionRequest:
					{
						INonAllocTransitionEvent<TBaseState> @event;

						if (!events.TryGet(
							eventTransitionRequest.EventType,
							out @event))
						{
							throw new Exception(
								logger.TryFormatException(
									GetType(),
									$"EVENT {eventTransitionRequest.EventType.Name} NOT FOUND"));
						}

						PerformTransition(
							@event,
							transitionRequest);

						break;
					}

					case NonAllocImmediateTransitionRequest immediateTransitionRequest:
					{
						if (!states.TryGet(
							immediateTransitionRequest.TargetStateType,
							out var newState))
						{
							throw new Exception(
								logger.TryFormatException(
									GetType(),
									$"STATE {immediateTransitionRequest.TargetStateType.Name} NOT FOUND"));
						}

						var previousState = currentState;

						PerformTransition(
							previousState,
							newState,
							transitionRequest);

						break;
					}
				}
			}
		}

		#endregion

		#endregion

		protected void PerformTransition(
			INonAllocTransitionEvent<TBaseState> @event,
			INonAllocTransitionRequest transitionRequest)
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

			if (!states.TryGet(
				@event.To,
				out TBaseState newState))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"STATE {@event.To.Name} NOT FOUND"));
			}

			var publisher = onEventFired as IPublisherSingleArgGeneric<INonAllocTransitionEvent<TBaseState>>;

			publisher?.Publish(
				@event);

			var previousState = currentState;

			PerformTransition(
				previousState,
				newState,
				transitionRequest);
		}

		protected void PerformTransition(
			TBaseState previousState,
			TBaseState newState,
			INonAllocTransitionRequest transitionRequest)
		{
			transitionInProgress = true;

			if (transitionRequest != null)
			{
				transitionRequest.TransitionState = ETransitionState.IN_PROGRESS;
			}

			#region Exit previous state

			object[] args = new object[]
			{
				previousState,
				newState
			};

			var stateChangeStartPublisher = onCurrentStateChangeStarted
				as IPublisherMultipleArgs;

			stateChangeStartPublisher?.Publish(
				args);

			if (transitionRequest != null)
				transitionController.ExitState(
					previousState,
					transitionRequest);
			else
				transitionController.ExitState(
					previousState);

			#endregion

			currentState = newState;

			#region Enter new state

			if (transitionRequest != null)
				transitionController.EnterState(
					newState,
					transitionRequest);
			else
				transitionController.EnterState(
					newState);

			var stateChangeFinishPublisher = onCurrentStateChangeFinished
				as IPublisherMultipleArgs;

			stateChangeFinishPublisher?.Publish(
				args);

			#endregion

			if (transitionRequest != null)
			{
				transitionRequest.TransitionState = ETransitionState.COMPLETED;
			}

			transitionInProgress = false;
		}
	}
}