using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates.NonAlloc;

namespace HereticalSolutions.FSM.NonAlloc.Concurrent
{
	public class ConcurrentBaseNonAllocStateMachine<TBaseState>
		: INonAllocStateMachine<TBaseState>
		where TBaseState : INonAllocState
	{
		private readonly INonAllocStateMachine<TBaseState> stateMachine;

		private readonly object lockObject;

		public ConcurrentBaseNonAllocStateMachine(
			INonAllocStateMachine<TBaseState> stateMachine,
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

		public INonAllocSubscribable OnCurrentStateChangeStarted =>
			stateMachine.OnCurrentStateChangeStarted;

		public INonAllocSubscribable OnCurrentStateChangeFinished =>
			stateMachine.OnCurrentStateChangeFinished;

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

		#region Transition requests

		public INonAllocStateMachine<TBaseState> PopTransitionRequest<TEvent>(
			out NonAllocEventTransitionRequest transitionRequest)
		{
			lock (lockObject)
			{
				return stateMachine.PopTransitionRequest<TEvent>(
					out transitionRequest);
			}
		}

		public INonAllocStateMachine<TBaseState> PopTransitionRequest(
			Type eventType,
			out NonAllocEventTransitionRequest transitionRequest)
		{
			lock (lockObject)
			{
				return stateMachine.PopTransitionRequest(
					eventType,
					out transitionRequest);
			}
		}

		#endregion

		#region Immediate transition requests

		public INonAllocStateMachine<TBaseState> PopImmediateTransitionRequest<TState>(
			out NonAllocImmediateTransitionRequest transitionRequest)
		{
			lock (lockObject)
			{
				return stateMachine.PopImmediateTransitionRequest<TState>(
					out transitionRequest);
			}
		}

		public INonAllocStateMachine<TBaseState> PopImmediateTransitionRequest(
			Type stateType,
			out NonAllocImmediateTransitionRequest transitionRequest)
		{
			lock (lockObject)
			{
				return stateMachine.PopImmediateTransitionRequest(
					stateType,
					out transitionRequest);
			}
		}

		#endregion

		public void RecycleTransitionRequest(
			INonAllocTransitionRequest transitionRequest)
		{
			lock (lockObject)
			{
				stateMachine.RecycleTransitionRequest(
					transitionRequest);
			}
		}

		#region Event handling

		public bool Handle<TEvent>(
			bool queueIfTransitionInProgress = true,
			bool processQueueAfterFinish = true)
			where TEvent : INonAllocTransitionEvent<TBaseState>
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

		public INonAllocSubscribable OnEventFired => stateMachine.OnEventFired;

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
					processQueueAfterFinish);
			}
		}

		#endregion

		#region Scheduled transition

		public IEnumerable<INonAllocTransitionRequest> ScheduledTransitions => stateMachine.ScheduledTransitions;

		public void ScheduleTransition(
			INonAllocTransitionRequest request,
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