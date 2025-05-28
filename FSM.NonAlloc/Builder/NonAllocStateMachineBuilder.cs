using System;

using HereticalSolutions.Repositories;

using HereticalSolutions.Builders;

using HereticalSolutions.FSM.NonAlloc.Factories;

namespace HereticalSolutions.FSM.NonAlloc.Builders
{
	public class NonAllocStateMachineBuilder<TBaseState>
		: ABuilder<NonAllocStateMachineBuilderContext<TBaseState>>
		where TBaseState : INonAllocState
	{
		public NonAllocStateMachineBuilder()
			: base()
		{
		}

		public NonAllocStateMachineBuilder<TBaseState> New(
			IRepository<Type, TBaseState> states,
			IRepository<Type, INonAllocTransitionEvent<TBaseState>> events,
			INonAllocTransitionController<TBaseState> transitionController)
		{
			context = new NonAllocStateMachineBuilderContext<TBaseState>
			{
				States = states,
				Events = events,
				TransitionController = transitionController
			};

			return this;
		}

		public NonAllocStateMachineBuilder<TBaseState> AddState<T>(
			T state)
			where T : TBaseState
		{
			context.States.Add(
				typeof(T),
				state);

			return this;
		}

		public T GetState<T>()
			where T : TBaseState
		{
			return (T)context.States[typeof(T)];
		}

		public IRepository<Type, TBaseState> States => context.States;

		public NonAllocStateMachineBuilder<TBaseState> AddTransitionEvent<TEvent>(
			TEvent transitionEvent)
			where TEvent : INonAllocTransitionEvent<TBaseState>
		{
			context.Events.Add(
				typeof(TEvent),
				transitionEvent);

			return this;
		}

		public IRepository<Type, INonAllocTransitionEvent<TBaseState>> Events =>
			context.Events;

		public BaseNonAllocStateMachine<TBaseState> 
			BuildBaseNonAllocStateMachine<TInitialState>(
				NonAllocFSMFactory nonAllocFSMFactory)
				where TInitialState : TBaseState
		{
			var result = nonAllocFSMFactory
				.BuildBaseNonAllocStateMachine<TBaseState, TInitialState>(
				context.States,
				context.Events,
				context.TransitionController);

			Cleanup();

			return result;
		}
	}
}