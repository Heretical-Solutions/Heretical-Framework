using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Async;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Delegates.NonAlloc.Factories;

using HereticalSolutions.ObjectPools.Configurable.Async.Factories;

using HereticalSolutions.FSM.NonAlloc.Async.Builders;

using HereticalSolutions.Logging;

namespace HereticalSolutions.FSM.NonAlloc.Async.Factories
{
	public class AsyncNonAllocFSMFactory
	{
		private readonly RepositoryFactory repositoryFactory;

		private readonly AsyncConfigurableStackPoolFactory
			asyncConfigurableStackPoolFactory;

		private readonly NonAllocBroadcasterFactory nonAllocBroadcasterFactory;

		private readonly ILoggerResolver loggerResolver;

		#region Request pool

		private const int
			DEFAULT_REQUEST_POOL_INITIAL_ALLOCATION_AMOUNT = 8;

		private const int
			DEFAULT_REQUEST_POOL_ADDITIONAL_ALLOCATION_AMOUNT = 8;

		protected AllocationCommandDescriptor
			defaultRequestPoolInitialAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = DEFAULT_REQUEST_POOL_INITIAL_ALLOCATION_AMOUNT
				};

		protected AllocationCommandDescriptor
			defaultRequestPoolAdditionalAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = DEFAULT_REQUEST_POOL_ADDITIONAL_ALLOCATION_AMOUNT
				};

		#endregion

		public AsyncNonAllocFSMFactory(
			RepositoryFactory repositoryFactory,
			AsyncConfigurableStackPoolFactory asyncConfigurableStackPoolFactory,
			NonAllocBroadcasterFactory nonAllocBroadcasterFactory,
			ILoggerResolver loggerResolver)
		{
			this.repositoryFactory = repositoryFactory;

			this.asyncConfigurableStackPoolFactory =
				asyncConfigurableStackPoolFactory;

			this.nonAllocBroadcasterFactory = nonAllocBroadcasterFactory;

			this.loggerResolver = loggerResolver;

			defaultRequestPoolInitialAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = DEFAULT_REQUEST_POOL_INITIAL_ALLOCATION_AMOUNT
				};

			defaultRequestPoolAdditionalAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = DEFAULT_REQUEST_POOL_ADDITIONAL_ALLOCATION_AMOUNT
				};
		}

		public AsyncNonAllocFSMFactory(
			RepositoryFactory repositoryFactory,
			AsyncConfigurableStackPoolFactory asyncConfigurableStackPoolFactory,
			NonAllocBroadcasterFactory nonAllocBroadcasterFactory,

			AllocationCommandDescriptor
				defaultRequestPoolInitialAllocationDescriptor,
			AllocationCommandDescriptor
				defaultRequestPoolAdditionalAllocationDescriptor,

			ILoggerResolver loggerResolver)
		{
			this.repositoryFactory = repositoryFactory;

			this.asyncConfigurableStackPoolFactory =
				asyncConfigurableStackPoolFactory;

			this.nonAllocBroadcasterFactory = nonAllocBroadcasterFactory;

			this.loggerResolver = loggerResolver;

			this.defaultRequestPoolInitialAllocationDescriptor = 
				defaultRequestPoolInitialAllocationDescriptor;

			this.defaultRequestPoolAdditionalAllocationDescriptor = 
				defaultRequestPoolAdditionalAllocationDescriptor;
		}

		public AsyncNonAllocStateMachineBuilder<TBaseState>
			BuildAsyncNonAllocStateMachineBuilder<TBaseState>()
			where TBaseState : IAsyncNonAllocState
		{
			return new AsyncNonAllocStateMachineBuilder<TBaseState>(
				repositoryFactory);
		}

		public async Task<BaseAsyncNonAllocStateMachine<TBaseState>>
			BuildBaseAsyncNonAllocStateMachine<TBaseState, TInitialState>(
				IRepository<Type, TBaseState> states,
				IRepository<Type, IAsyncNonAllocTransitionEvent<TBaseState>> events,
				IAsyncNonAllocTransitionController<TBaseState> transitionController,

				//Async tail
				AsyncExecutionContext asyncContext)
			where TInitialState : TBaseState
			where TBaseState : IAsyncNonAllocState
		{
			return await BuildBaseAsyncNonAllocStateMachine<TBaseState, TInitialState>(
				states,
				events,
				transitionController,

				defaultRequestPoolInitialAllocationDescriptor,
				defaultRequestPoolAdditionalAllocationDescriptor,

				asyncContext);
		}

		public async Task<BaseAsyncNonAllocStateMachine<TBaseState>>
			BuildBaseAsyncNonAllocStateMachine<TBaseState, TInitialState>(
				IRepository<Type, TBaseState> states,
				IRepository<Type, IAsyncNonAllocTransitionEvent<TBaseState>> events,
				IAsyncNonAllocTransitionController<TBaseState> transitionController,

				AllocationCommandDescriptor initial,
				AllocationCommandDescriptor additional,
				
				//Async tail
				AsyncExecutionContext asyncContext)
			where TInitialState : TBaseState
			where TBaseState : IAsyncNonAllocState
		{
			var logger =
				loggerResolver?.GetLogger<BaseAsyncNonAllocStateMachine<TBaseState>>();

			Func<Task<AsyncNonAllocEventTransitionRequest>>
				eventTransitionAllocationDelegate =
					BuildAsyncNonAllocEventTransitionRequest<TBaseState>;

			Func<Task<AsyncNonAllocImmediateTransitionRequest>>
				immediateTransitionAllocationDelegate =
					BuildAsyncNonAllocImmediateTransitionRequest<TBaseState>;

			var eventPool = await asyncConfigurableStackPoolFactory.	
				BuildAsyncConfigurableStackPool<
					AsyncNonAllocEventTransitionRequest>(
						new AsyncAllocationCommand<AsyncNonAllocEventTransitionRequest>(
							initial,
							eventTransitionAllocationDelegate,
							null),
							new AsyncAllocationCommand<AsyncNonAllocEventTransitionRequest>(
							additional,
							eventTransitionAllocationDelegate,
							null),

							asyncContext);

			var immediatePool = await asyncConfigurableStackPoolFactory.
				BuildAsyncConfigurableStackPool<
					AsyncNonAllocImmediateTransitionRequest>(
						new AsyncAllocationCommand<AsyncNonAllocImmediateTransitionRequest>(
							initial,
							immediateTransitionAllocationDelegate,
							null),
							new AsyncAllocationCommand<AsyncNonAllocImmediateTransitionRequest>(
							additional,
							immediateTransitionAllocationDelegate,
							null),

							asyncContext);

			return new BaseAsyncNonAllocStateMachine<TBaseState>(
				states,
				events,

				eventPool,
				immediatePool,

				transitionController,
				new Queue<IAsyncNonAllocTransitionRequest>(),

				nonAllocBroadcasterFactory.BuildNonAllocBroadcasterMultipleArgs(),
				nonAllocBroadcasterFactory.BuildNonAllocBroadcasterMultipleArgs(),
				nonAllocBroadcasterFactory.
					BuildNonAllocBroadcasterGeneric<
						IAsyncNonAllocTransitionEvent<TBaseState>>(),

				states[typeof(TInitialState)],

				new object(),

				logger);
		}

		public async Task<AsyncNonAllocEventTransitionRequest>
			BuildAsyncNonAllocEventTransitionRequest<TBaseState>()
		{
			return new AsyncNonAllocEventTransitionRequest(
				new object(),

				nonAllocBroadcasterFactory
					.BuildNonAllocBroadcasterGeneric<TBaseState>(),
				nonAllocBroadcasterFactory
					.BuildNonAllocBroadcasterGeneric<TBaseState>());
		}

		public async Task<AsyncNonAllocImmediateTransitionRequest>
			BuildAsyncNonAllocImmediateTransitionRequest<TBaseState>()
		{
			return new AsyncNonAllocImmediateTransitionRequest(
				new object(),

				nonAllocBroadcasterFactory
					.BuildNonAllocBroadcasterGeneric<TBaseState>(),
				nonAllocBroadcasterFactory
					.BuildNonAllocBroadcasterGeneric<TBaseState>());
		}
	}
}