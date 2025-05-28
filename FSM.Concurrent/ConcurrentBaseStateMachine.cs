using System;
using System.Collections.Generic;

namespace HereticalSolutions.FSM.Concurrent
{
	public class ConcurrentBaseStateMachine<TBaseState>
		: IStateMachine<TBaseState>
		where TBaseState : IState
	{
		private readonly IStateMachine<TBaseState> stateMachine;

		private readonly object lockObject;

		public ConcurrentBaseStateMachine(
			IStateMachine<TBaseState> stateMachine,
			object lockObject)
		{
			this.stateMachine = stateMachine;

			this.lockObject = lockObject;
		}

		#region IStateMachine

		public bool TransitionInProgress
		{
			get
			{
				lock (lockObject)
				{
					return stateMachine.TransitionInProgress;
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
					return stateMachine.CurrentState;
				}
			}
		}

		public Action<TBaseState, TBaseState> OnCurrentStateChangeStarted
		{
			get
			{
				lock (lockObject)
				{
					return stateMachine.OnCurrentStateChangeStarted;
				}
			}
			set
			{
				lock (lockObject)
				{
					stateMachine.OnCurrentStateChangeStarted = value;
				}
			}
		}

		public Action<TBaseState, TBaseState> OnCurrentStateChangeFinished
		{
			get
			{
				lock (lockObject)
				{
					return stateMachine.OnCurrentStateChangeFinished;
				}
			}
			set
			{
				lock (lockObject)
				{
					stateMachine.OnCurrentStateChangeFinished = value;
				}
			}
		}

		#endregion

		#region All states

		public TConcreteState GetState<TConcreteState>()
			where TConcreteState : TBaseState
		{
			return stateMachine.GetState<TConcreteState>();
		}

		public TBaseState GetState(
			Type stateType)
		{
			return stateMachine.GetState(
				stateType);
		}

		public IEnumerable<Type> AllStates
		{
			get => stateMachine.AllStates;
		}

		#endregion

		#region Event handling

		public bool Handle<TEvent>(
			bool queueIfTransitionInProgress = true,
			bool processQueueAfterFinish = true)
			where TEvent : ITransitionEvent<TBaseState>
		{
			lock (lockObject)
			{
				return stateMachine.Handle<TEvent>(
					queueIfTransitionInProgress,
					processQueueAfterFinish);
			}
		}

		public bool Handle(
			Type eventType,
			bool queueIfTransitionInProgress = true,
			bool processQueueAfterFinish = true)
		{
			lock (lockObject)
			{
				return stateMachine.Handle(
					eventType,
					queueIfTransitionInProgress,
					processQueueAfterFinish);
			}
		}

		public Action<ITransitionEvent<TBaseState>> OnEventFired => stateMachine.OnEventFired;

		#endregion

		#region Immediate transition

		public bool TransitToImmediately<TState>(
			bool queueIfTransitionInProgress = true,
			bool processQueueAfterFinish = true)
			where TState : TBaseState
		{
			lock (lockObject)
			{
				return stateMachine.TransitToImmediately<TState>(
					queueIfTransitionInProgress,
					processQueueAfterFinish);
			}
		}

		public bool TransitToImmediately(
			Type stateType,
			bool queueIfTransitionInProgress = true,
			bool processQueueAfterFinish = true)
		{
			lock (lockObject)
			{
				return stateMachine.TransitToImmediately(
					stateType,
					queueIfTransitionInProgress,
					processQueueAfterFinish);
			}
		}

		#endregion

		#region Scheduled transition

		public IEnumerable<ITransitionRequest> ScheduledTransitions => stateMachine.ScheduledTransitions;

		public void ScheduleTransition(
			ITransitionRequest request,
			bool startProcessingIfIdle = true)
		{
			lock (lockObject)
			{
				stateMachine.ScheduleTransition(
					request,
					startProcessingIfIdle);
			}
		}

		public void ProcessTransitionQueue()
		{
			lock (lockObject)
			{
				stateMachine.ProcessTransitionQueue();
			}
		}

		#endregion

		#endregion
	}
}