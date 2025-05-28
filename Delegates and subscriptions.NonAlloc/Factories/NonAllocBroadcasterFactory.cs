using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.ObjectPools;
using HereticalSolutions.ObjectPools.Configurable.Factories;

using HereticalSolutions.Bags;
using HereticalSolutions.Bags.NonAlloc.Factories;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.NonAlloc.Factories
{
	public class NonAllocBroadcasterFactory
	{
		private readonly ConfigurableStackPoolFactory
			configurableStackPoolFactory;

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

		public NonAllocBroadcasterFactory(
			ConfigurableStackPoolFactory
				configurableStackPoolFactory,
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

		public NonAllocBroadcasterFactory(
			ConfigurableStackPoolFactory
				configurableStackPoolFactory,
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

		#region Non alloc broadcaster generic

		public NonAllocBroadcasterGeneric<T>
			BuildNonAllocBroadcasterGeneric<T>()
		{
			return BuildNonAllocBroadcasterGeneric<T>(
				defaultSubscriptionPoolInitialAllocationDescriptor,
				defaultSubscriptionPoolAdditionalAllocationDescriptor);
		}

		public NonAllocBroadcasterGeneric<T>
			BuildNonAllocBroadcasterGeneric<T>(
				AllocationCommandDescriptor initial,
				AllocationCommandDescriptor additional)
		{
			Func<IBag<INonAllocSubscription>>
				invocationContextAllocationDelegate =
					nonAllocLinkedListBagFactory.
						BuildNonAllocLinkedListBag<INonAllocSubscription>;

			return BuildNonAllocBroadcasterGeneric<T>(
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

		public NonAllocBroadcasterGeneric<T>
			BuildNonAllocBroadcasterGeneric<T>(
				IBag<INonAllocSubscription> subscriptionsBag,
				IPool<IBag<INonAllocSubscription>> contextPool)
		{
			ILogger logger =
				loggerResolver?.GetLogger<NonAllocBroadcasterGeneric<T>>();

			return new NonAllocBroadcasterGeneric<T>(
				subscriptionsBag,
				contextPool,
				logger);
		}

		#endregion

		#region Non alloc broadcaster multiple args

		public NonAllocBroadcasterMultipleArgs
			BuildNonAllocBroadcasterMultipleArgs()
		{
			return BuildNonAllocBroadcasterMultipleArgs(
				defaultSubscriptionPoolInitialAllocationDescriptor,
				defaultSubscriptionPoolAdditionalAllocationDescriptor);
		}

		public NonAllocBroadcasterMultipleArgs
			BuildNonAllocBroadcasterMultipleArgs(
				AllocationCommandDescriptor initial,
				AllocationCommandDescriptor additional)
		{
			Func<IBag<INonAllocSubscription>>
				invocationContextAllocationDelegate =
					nonAllocLinkedListBagFactory.
						BuildNonAllocLinkedListBag<INonAllocSubscription>;

			return BuildNonAllocBroadcasterMultipleArgs(
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

		public NonAllocBroadcasterMultipleArgs
			BuildNonAllocBroadcasterMultipleArgs(
				IBag<INonAllocSubscription> subscriptionsBag,
				IPool<IBag<INonAllocSubscription>> contextPool)
		{
			ILogger logger =
				loggerResolver?.GetLogger<NonAllocBroadcasterMultipleArgs>();

			return new NonAllocBroadcasterMultipleArgs(
				subscriptionsBag,
				contextPool,
				logger);
		}

		#endregion

		#region Non alloc broadcaster with repository

		public NonAllocBroadcasterWithRepository
			BuildNonAllocBroadcasterWithRepository(
				IRepository<Type, object> broadcasterRepository)
		{
			ILogger logger =
				loggerResolver?.GetLogger<NonAllocBroadcasterWithRepository>();

			return new NonAllocBroadcasterWithRepository(
				broadcasterRepository,
				logger);
		}

		#endregion
	}
}