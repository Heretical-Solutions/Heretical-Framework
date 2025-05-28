using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Builders;

using HereticalSolutions.FSM.NonAlloc.Async.Factories;

namespace HereticalSolutions.FSM.NonAlloc.Async.Builders
{
	public class AsyncNonAllocStateMachineBuilder<TBaseState>
		: ABuilder<AsyncNonAllocStateMachineBuilderContext<TBaseState>>
		where TBaseState : IAsyncNonAllocState
	{
		private readonly RepositoryFactory repositoryFactory;

		public AsyncNonAllocStateMachineBuilder(
			RepositoryFactory repositoryFactory)
		{
			this.repositoryFactory = repositoryFactory;
		}

		public AsyncNonAllocStateMachineBuilder<TBaseState> New()
		{
			context =
				new AsyncNonAllocStateMachineBuilderContext<TBaseState>
				{
					States = repositoryFactory
						.BuildDictionaryRepository<Type, TBaseState>(),
					Events = repositoryFactory
						.BuildDictionaryRepository<Type, IAsyncNonAllocTransitionEvent<TBaseState>>(),
					TransitionController =
						new BasicAsyncNonAllocTransitionController<TBaseState>()
				};

			return this;
		}

		public AsyncNonAllocStateMachineBuilder<TBaseState> AddState<T>(
			T state)
			where T : TBaseState
		{
			States.Add(
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

		public AsyncNonAllocStateMachineBuilder<TBaseState> AddTransitionEvent<TEvent>(
			TEvent transitionEvent)
			where TEvent : IAsyncNonAllocTransitionEvent<TBaseState>
		{
			context.Events.Add(
				typeof(TEvent),
				transitionEvent);

			return this;
		}

		public IRepository<Type, IAsyncNonAllocTransitionEvent<TBaseState>> Events =>
			context.Events;

		public async Task<BaseAsyncNonAllocStateMachine<TBaseState>> Build<TInitialState>(
			AsyncNonAllocFSMFactory asyncNonAllocFSMFactory,

			//Async tail
			AsyncExecutionContext asyncContext)
			where TInitialState : TBaseState
		{
			var result = await asyncNonAllocFSMFactory
				.BuildBaseAsyncNonAllocStateMachine<TBaseState, TInitialState>(
					context.States,
					context.Events,
					context.TransitionController,
					
					asyncContext);

			Cleanup();

			return result;
		}
	}
}