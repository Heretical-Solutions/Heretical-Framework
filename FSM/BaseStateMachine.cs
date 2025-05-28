using System;
using System.Collections.Generic;

namespace HereticalSolutions.FSM
{
	public class BaseStateMachine<TBaseState>
		: IStateMachine<TBaseState>
		where TBaseState : IState
	{
		protected readonly Dictionary<Type, TBaseState> states;

		protected readonly Dictionary<Type, ITransitionEvent<TBaseState>> events;

		protected readonly ITransitionController<TBaseState> transitionController;

		protected readonly Queue<ITransitionRequest> transitionQueue;


		protected Action<TBaseState, TBaseState> onCurrentStateChangeStarted;

		protected Action<TBaseState, TBaseState> onCurrentStateChangeFinished;

		protected Action<ITransitionEvent<TBaseState>> onEventFired;


		protected bool transitionInProgress;

		protected TBaseState currentState;


		public BaseStateMachine(
			Dictionary<Type, TBaseState> states,
			Dictionary<Type, ITransitionEvent<TBaseState>> events,

			ITransitionController<TBaseState> transitionController,
			Queue<ITransitionRequest> transitionQueue,

			TBaseState initialState)
		{
			this.states = states;

			this.events = events;


			this.transitionController = transitionController;

			this.transitionQueue = transitionQueue;


			onCurrentStateChangeStarted = null;
			
			onCurrentStateChangeFinished = null;

			onEventFired = null;
			

			currentState = initialState;

			transitionInProgress = false;
		}

		#region IStateMachine

		public bool TransitionInProgress => transitionInProgress;

		#region Current state

		public TBaseState CurrentState => currentState;

		public Action<TBaseState, TBaseState> OnCurrentStateChangeStarted
		{
			get => onCurrentStateChangeStarted;
			set => onCurrentStateChangeStarted = value;
		}

		public Action<TBaseState, TBaseState> OnCurrentStateChangeFinished
		{
			get => onCurrentStateChangeFinished;
			set => onCurrentStateChangeFinished = value;
		}

		#endregion

		#region All states

		public TConcreteState GetState<TConcreteState>()
			where TConcreteState : TBaseState
		{
			if (!states.TryGetValue(
				typeof(TConcreteState),
				out var result))
			{
				throw new Exception(
					$"STATE {nameof(TConcreteState)} NOT FOUND");
			}

			return (TConcreteState)result;
		}

		public TBaseState GetState(
			Type stateType)
		{
			if (!states.TryGetValue(
				stateType,
				out var result))
			{
				throw new Exception(
					$"STATE {stateType.Name} NOT FOUND");
			}

			return result;
		}

		public IEnumerable<Type> AllStates
		{
			get => states.Keys;
		}

		#endregion

		#region Event handling

		public bool Handle<TEvent>(
			bool queueIfTransitionInProgress = true,
			bool processQueueAfterFinish = true)
			where TEvent : ITransitionEvent<TBaseState>
		{
			if (transitionInProgress
			    || transitionQueue.Count != 0)
			{
				if (!queueIfTransitionInProgress)
				{
					return false;
				}
				
				ScheduleTransition(
					new EventTransitionRequest(
						typeof(TEvent)));

				return true;
			}

			ITransitionEvent<TBaseState> @event;

			if (!events.TryGetValue(
				typeof(TEvent),
				out @event))
			{
				throw new Exception(
					$"EVENT {nameof(TEvent)} NOT FOUND");
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
				
				ScheduleTransition(
					new EventTransitionRequest(
						eventType));

				return true;
			}

			ITransitionEvent<TBaseState> @event;

			if (!events.TryGetValue(
				eventType,
				out @event))
			{
				throw new Exception(
					$"EVENT {eventType.Name} NOT FOUND");
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

		public Action<ITransitionEvent<TBaseState>> OnEventFired
		{
			get => onEventFired;
			set => onEventFired = value;
		}

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
				
				ScheduleTransition(
					new ImmediateTransitionRequest(
						typeof(TState)));

				return true;
			}

			if (!states.TryGetValue(
				typeof(TState),
				out var newState))
			{
				throw new Exception(
					$"STATE {nameof(TState)} NOT FOUND");
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
				
				ScheduleTransition(
					new ImmediateTransitionRequest(
						stateType));

				return true;
			}

			if (!states.TryGetValue(
				stateType,
				out var newState))
				throw new Exception(
					$"STATE {stateType.Name} NOT FOUND");

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
	
		public IEnumerable<ITransitionRequest> ScheduledTransitions => transitionQueue;
	
		public void ScheduleTransition(
			ITransitionRequest request,
			bool startProcessingIfIdle = true)
		{
			if (request.TransitionState != ETransitionState.UNINITIALISED)
			{
				throw new Exception(
					$"TRANSITION REQUEST {request.GetType().Name} ALREADY SCHEDULED");
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
					case EventTransitionRequest eventTransitionRequest:
					{
						ITransitionEvent<TBaseState> @event;

						if (!events.TryGetValue(
							eventTransitionRequest.EventType,
							out @event))
						{
							throw new Exception(
								$"EVENT {eventTransitionRequest.EventType.Name} NOT FOUND");
						}

						PerformTransition(
							@event,
							transitionRequest);

						break;
					}

					case ImmediateTransitionRequest immediateTransitionRequest:
					{
						if (!states.TryGetValue(
							immediateTransitionRequest.TargetStateType,
							out var newState))
						{
							throw new Exception(
								$"STATE {immediateTransitionRequest.TargetStateType.Name} NOT FOUND");
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

		private void PerformTransition(
			ITransitionEvent<TBaseState> @event,
			ITransitionRequest transitionRequest)
		{
			if (currentState.GetType() != @event.From)
			{
				string currentStateString = currentState.GetType().Name;

				string transitionString = @event.GetType().Name;

				string fromStateString = @event.From.GetType().Name;

				throw new Exception(
					$"CURRENT STATE {currentStateString} IS NOT EQUAL TO TRANSITION {transitionString} PREVIOUS STATE {fromStateString}");
			}

			if (!states.TryGetValue(
				@event.To,
				out TBaseState newState))
			{
				throw new Exception(
					$"STATE {@event.To.Name} NOT FOUND");
			}

			onEventFired?.Invoke(
				@event);

			var previousState = currentState;

			PerformTransition(
				previousState,
				newState,
				transitionRequest);
		}

		private void PerformTransition(
			TBaseState previousState,
			TBaseState newState,
			ITransitionRequest transitionRequest)
		{
			transitionInProgress = true;

			if (transitionRequest != null)
			{
				transitionRequest.TransitionState = ETransitionState.IN_PROGRESS;
			}

			#region Exit previous state

			onCurrentStateChangeStarted?.Invoke(
				previousState,
				newState);

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

			onCurrentStateChangeFinished?.Invoke(
				previousState,
				newState);

			#endregion

			if (transitionRequest != null)
			{
				transitionRequest.TransitionState = ETransitionState.COMPLETED;
			}

			transitionInProgress = false;
		}
	}
} 