using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.ObjectPools;
using HereticalSolutions.ObjectPools.Configurable.Factories;

using HereticalSolutions.Bags;
using HereticalSolutions.Bags.NonAlloc.Factories;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.NonAlloc.Concurrent.Factories
{
	public class ConcurrentNonAllocBroadcasterFactory
	{
		private readonly ConfigurableStackPoolFactory configurableStackPoolFactory;

		private readonly NonAllocLinkedListBagFactory nonAllocLinkedListBagFactory;

		private readonly ILoggerResolver loggerResolver;

		#region Broadcaster subscriptions pool

		private const int
			DEFAULT_SUBSCRIPTION_POOL_INITIAL_ALLOCATION_AMOUNT = 8;

		private const int
			DEFAULT_SUBSCRIPTION_POOL_ADDITIONAL_ALLOCATION_AMOUNT = 8;

		protected AllocationCommandDescriptor
			defaultSubscriptionPoolInitialAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = DEFAULT_SUBSCRIPTION_POOL_INITIAL_ALLOCATION_AMOUNT
				};

		protected AllocationCommandDescriptor
			defaultSubscriptionPoolAdditionalAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = DEFAULT_SUBSCRIPTION_POOL_ADDITIONAL_ALLOCATION_AMOUNT
				};

		#endregion

		#region Invocation context

		private const int DEFAULT_INVOKATION_CONTEXT_SIZE = 32;

		protected int invokationContextSize = DEFAULT_INVOKATION_CONTEXT_SIZE;

		#endregion

		public ConcurrentNonAllocBroadcasterFactory(
			ConfigurableStackPoolFactory configurableStackPoolFactory,
			NonAllocLinkedListBagFactory nonAllocLinkedListBagFactory,
			ILoggerResolver loggerResolver)
		{
			this.configurableStackPoolFactory = configurableStackPoolFactory;

			this.nonAllocLinkedListBagFactory = nonAllocLinkedListBagFactory;

			this.loggerResolver = loggerResolver;

			defaultSubscriptionPoolInitialAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = DEFAULT_SUBSCRIPTION_POOL_INITIAL_ALLOCATION_AMOUNT
				};

			defaultSubscriptionPoolAdditionalAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = DEFAULT_SUBSCRIPTION_POOL_ADDITIONAL_ALLOCATION_AMOUNT
				};


			invokationContextSize = DEFAULT_INVOKATION_CONTEXT_SIZE;
		}

		public ConcurrentNonAllocBroadcasterFactory(
			ConfigurableStackPoolFactory configurableStackPoolFactory,
			NonAllocLinkedListBagFactory nonAllocLinkedListBagFactory,

			AllocationCommandDescriptor
				defaultSubscriptionPoolInitialAllocationDescriptor,
			AllocationCommandDescriptor
				defaultSubscriptionPoolAdditionalAllocationDescriptor,

			int invokationContextSize,

			ILoggerResolver loggerResolver)
		{
			this.configurableStackPoolFactory = configurableStackPoolFactory;

			this.nonAllocLinkedListBagFactory = nonAllocLinkedListBagFactory;


			this.defaultSubscriptionPoolInitialAllocationDescriptor =
				defaultSubscriptionPoolInitialAllocationDescriptor;

			this.defaultSubscriptionPoolAdditionalAllocationDescriptor =
				defaultSubscriptionPoolAdditionalAllocationDescriptor;


			this.invokationContextSize = invokationContextSize;


			this.loggerResolver = loggerResolver;
		}

		#region Concurrent non alloc broadcaster generic

		public ConcurrentNonAllocBroadcasterGeneric<T>
			BuildConcurrentNonAllocBroadcasterGeneric<T>()
		{
			return BuildConcurrentNonAllocBroadcasterGeneric<T>(
				defaultSubscriptionPoolInitialAllocationDescriptor,
				defaultSubscriptionPoolAdditionalAllocationDescriptor);
		}

		public ConcurrentNonAllocBroadcasterGeneric<T>
			BuildConcurrentNonAllocBroadcasterGeneric<T>(
				AllocationCommandDescriptor initial,
				AllocationCommandDescriptor additional)
		{
			Func<IBag<INonAllocSubscription>> invocationContextAllocationDelegate =
				nonAllocLinkedListBagFactory.
					BuildNonAllocLinkedListBag<INonAllocSubscription>;

			return BuildConcurrentNonAllocBroadcasterGeneric<T>(
				nonAllocLinkedListBagFactory.
					BuildNonAllocLinkedListBag<INonAllocSubscription>(),
				configurableStackPoolFactory.
					BuildConfigurableStackPool<IBag<INonAllocSubscription>>(
						new AllocationCommand<IBag<INonAllocSubscription>>(
							initial,
							invocationContextAllocationDelegate,
							null),
						new AllocationCommand<IBag<INonAllocSubscription>>(
							additional,
							invocationContextAllocationDelegate,
							null)));
		}

		public ConcurrentNonAllocBroadcasterGeneric<T>
			BuildConcurrentNonAllocBroadcasterGeneric<T>(
				IBag<INonAllocSubscription> subscriptionsBag,
				IPool<IBag<INonAllocSubscription>> contextPool)
		{
			ILogger logger =
				loggerResolver?.GetLogger<ConcurrentNonAllocBroadcasterGeneric<T>>();

			return new ConcurrentNonAllocBroadcasterGeneric<T>(
				subscriptionsBag,
				contextPool,
				new object(),
				logger);
		}

		#endregion

		#region Concurrent non alloc broadcaster multiple args

		public ConcurrentNonAllocBroadcasterMultipleArgs
			BuildConcurrentNonAllocBroadcasterMultipleArgs()
		{
			return BuildConcurrentNonAllocBroadcasterMultipleArgs(
				defaultSubscriptionPoolInitialAllocationDescriptor,
				defaultSubscriptionPoolAdditionalAllocationDescriptor);
		}

		public ConcurrentNonAllocBroadcasterMultipleArgs
			BuildConcurrentNonAllocBroadcasterMultipleArgs(
				AllocationCommandDescriptor initial,
				AllocationCommandDescriptor additional)
		{
			Func<IBag<INonAllocSubscription>>
				invocationContextAllocationDelegate =
					nonAllocLinkedListBagFactory.
					BuildNonAllocLinkedListBag<INonAllocSubscription>;

			return BuildConcurrentNonAllocBroadcasterMultipleArgs(
				nonAllocLinkedListBagFactory.
					BuildNonAllocLinkedListBag<INonAllocSubscription>(),
				configurableStackPoolFactory.
					BuildConfigurableStackPool<IBag<INonAllocSubscription>>(
						new AllocationCommand<IBag<INonAllocSubscription>>(
							initial,
							invocationContextAllocationDelegate,
							null),
						new AllocationCommand<IBag<INonAllocSubscription>>(
							additional,
							invocationContextAllocationDelegate,
							null)));
		}

		public ConcurrentNonAllocBroadcasterMultipleArgs
			BuildConcurrentNonAllocBroadcasterMultipleArgs(
				IBag<INonAllocSubscription> subscriptionsBag,
				IPool<IBag<INonAllocSubscription>> contextPool)
		{
			ILogger logger =
				loggerResolver?.GetLogger<ConcurrentNonAllocBroadcasterMultipleArgs>();

			return new ConcurrentNonAllocBroadcasterMultipleArgs(
				subscriptionsBag,
				contextPool,
				new object(),
				logger);
		}

		#endregion

		#region Concurrent non alloc broadcaster with repository

		public ConcurrentNonAllocBroadcasterWithRepository
			BuildConcurrentNonAllocBroadcasterWithRepository(
				IRepository<Type, object> broadcasterRepository)
		{
			ILogger logger =
				loggerResolver?.GetLogger<ConcurrentNonAllocBroadcasterWithRepository>();

			return new ConcurrentNonAllocBroadcasterWithRepository(
				broadcasterRepository,
				new object(),
				logger);
		}

		#endregion
	}
}