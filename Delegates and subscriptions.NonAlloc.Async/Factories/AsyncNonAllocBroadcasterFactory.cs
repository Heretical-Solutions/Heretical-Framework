using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.ObjectPools;
using HereticalSolutions.ObjectPools.Configurable.Factories;

using HereticalSolutions.Bags;
using HereticalSolutions.Bags.NonAlloc.Factories;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.NonAlloc.Async.Factories
{
	public class AsyncNonAllocBroadcasterFactory
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

		public AsyncNonAllocBroadcasterFactory(
			ConfigurableStackPoolFactory configurableStackPoolFactory,
			NonAllocLinkedListBagFactory nonAllocLinkedListBagFactory,
			ILoggerResolver loggerResolver)
		{
			this.configurableStackPoolFactory =
				configurableStackPoolFactory;

			this.nonAllocLinkedListBagFactory =
				nonAllocLinkedListBagFactory;

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

		public AsyncNonAllocBroadcasterFactory(
			ConfigurableStackPoolFactory configurableStackPoolFactory,
			NonAllocLinkedListBagFactory nonAllocLinkedListBagFactory,

			AllocationCommandDescriptor
				defaultSubscriptionPoolInitialAllocationDescriptor,
			AllocationCommandDescriptor
				defaultSubscriptionPoolAdditionalAllocationDescriptor,

			int invokationContextSize,

			ILoggerResolver loggerResolver)
		{
			this.configurableStackPoolFactory =
				configurableStackPoolFactory;

			this.nonAllocLinkedListBagFactory =
				nonAllocLinkedListBagFactory;


			this.defaultSubscriptionPoolInitialAllocationDescriptor =
				defaultSubscriptionPoolInitialAllocationDescriptor;

			this.defaultSubscriptionPoolAdditionalAllocationDescriptor =
				defaultSubscriptionPoolAdditionalAllocationDescriptor;


			this.invokationContextSize = invokationContextSize;


			this.loggerResolver = loggerResolver;
		}

		#region Async broadcaster generic

		public AsyncNonAllocBroadcasterGeneric<T>
			BuildAsyncNonAllocBroadcasterGeneric<T>()
		{
			return BuildAsyncNonAllocBroadcasterGeneric<T>(
				defaultSubscriptionPoolInitialAllocationDescriptor,
				defaultSubscriptionPoolAdditionalAllocationDescriptor);
		}

		public AsyncNonAllocBroadcasterGeneric<T>
			BuildAsyncNonAllocBroadcasterGeneric<T>(
				AllocationCommandDescriptor initial,
				AllocationCommandDescriptor additional)
		{
			Func<IBag<INonAllocSubscription>>
				invocationContextAllocationDelegate =
					nonAllocLinkedListBagFactory.
						BuildNonAllocLinkedListBag<INonAllocSubscription>;

			return BuildAsyncNonAllocBroadcasterGeneric<T>(
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

		public AsyncNonAllocBroadcasterGeneric<T>
			BuildAsyncNonAllocBroadcasterGeneric<T>(
				IBag<INonAllocSubscription> subscriptionsBag,
				IPool<IBag<INonAllocSubscription>> contextPool)
		{
			ILogger logger =
				loggerResolver?.GetLogger<AsyncNonAllocBroadcasterGeneric<T>>();

			return new AsyncNonAllocBroadcasterGeneric<T>(
				subscriptionsBag,
				contextPool,
				new object(),
				logger);
		}

		#endregion

		#region Async broadcaster multiple args

		public AsyncNonAllocBroadcasterMultipleArgs
			BuildAsyncNonAllocBroadcasterMultipleArgs()
		{
			return BuildAsyncNonAllocBroadcasterMultipleArgs(
				defaultSubscriptionPoolInitialAllocationDescriptor,
				defaultSubscriptionPoolAdditionalAllocationDescriptor);
		}

		public AsyncNonAllocBroadcasterMultipleArgs
			BuildAsyncNonAllocBroadcasterMultipleArgs(
				AllocationCommandDescriptor initial,
				AllocationCommandDescriptor additional)
		{
			Func<IBag<INonAllocSubscription>>
				invocationContextAllocationDelegate =
					nonAllocLinkedListBagFactory.
						BuildNonAllocLinkedListBag<INonAllocSubscription>;

			return BuildAsyncNonAllocBroadcasterMultipleArgs(
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

		public AsyncNonAllocBroadcasterMultipleArgs
			BuildAsyncNonAllocBroadcasterMultipleArgs(
				IBag<INonAllocSubscription> subscriptionsBag,
				IPool<IBag<INonAllocSubscription>> contextPool)
		{
			ILogger logger =
				loggerResolver?.GetLogger<NonAllocBroadcasterMultipleArgs>();

			return new AsyncNonAllocBroadcasterMultipleArgs(
				subscriptionsBag,
				contextPool,
				new object(),
				logger);
		}

		#endregion

		#region Async broadcaster with repository

		public AsyncNonAllocBroadcasterWithRepository 
			BuildAsyncNonAllocBroadcasterWithRepository(
				IRepository<Type, object> broadcasterRepository)
		{
			ILogger logger =
				loggerResolver?.GetLogger<AsyncNonAllocBroadcasterWithRepository>();

			return new AsyncNonAllocBroadcasterWithRepository(
				broadcasterRepository,
				new object(),
				logger);
		}

		#endregion
	}
}