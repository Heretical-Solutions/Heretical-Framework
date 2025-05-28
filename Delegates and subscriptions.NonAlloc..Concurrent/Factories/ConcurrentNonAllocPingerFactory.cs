using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.ObjectPools;
using HereticalSolutions.ObjectPools.Configurable.Factories;

using HereticalSolutions.Bags;
using HereticalSolutions.Bags.NonAlloc.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.NonAlloc.Concurrent.Factories
{
	public class ConcurrentNonAllocPingerFactory
	{
		private readonly PingerFactory pingerFactory;

		private readonly ConfigurableStackPoolFactory configurableStackPoolFactory;

		private readonly NonAllocLinkedListBagFactory nonAllocLinkedListBagFactory;

		private readonly ILoggerResolver loggerResolver;

		#region Pinger subscriptions pool

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

		public ConcurrentNonAllocPingerFactory(
			PingerFactory pingerFactory,
			ConfigurableStackPoolFactory configurableStackPoolFactory,
			NonAllocLinkedListBagFactory nonAllocLinkedListBagFactory,
			ILoggerResolver loggerResolver)
		{
			this.pingerFactory = pingerFactory;

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

		public ConcurrentNonAllocPingerFactory(
			PingerFactory pingerFactory,
			ConfigurableStackPoolFactory configurableStackPoolFactory,
			NonAllocLinkedListBagFactory nonAllocLinkedListBagFactory,

			AllocationCommandDescriptor
				defaultSubscriptionPoolInitialAllocationDescriptor,
			AllocationCommandDescriptor
				defaultSubscriptionPoolAdditionalAllocationDescriptor,

			int invokationContextSize,

			ILoggerResolver loggerResolver)
		{
			this.pingerFactory = pingerFactory;

			this.configurableStackPoolFactory = configurableStackPoolFactory;

			this.nonAllocLinkedListBagFactory = nonAllocLinkedListBagFactory;
			
			this.defaultSubscriptionPoolInitialAllocationDescriptor =
				defaultSubscriptionPoolInitialAllocationDescriptor;

			this.defaultSubscriptionPoolAdditionalAllocationDescriptor =
				defaultSubscriptionPoolAdditionalAllocationDescriptor;


			this.invokationContextSize = invokationContextSize;

			this.loggerResolver = loggerResolver;
		}

		#region Concurrent non alloc pinger

		public ConcurrentNonAllocPinger BuildConcurrentNonAllocPinger()
		{
			return BuildConcurrentNonAllocPinger(
				defaultSubscriptionPoolInitialAllocationDescriptor,
				defaultSubscriptionPoolAdditionalAllocationDescriptor);
		}

		public ConcurrentNonAllocPinger BuildConcurrentNonAllocPinger(
			AllocationCommandDescriptor initial,
			AllocationCommandDescriptor additional)
		{
			Func<IBag<INonAllocSubscription>> invocationContextAllocationDelegate =
				nonAllocLinkedListBagFactory.
					BuildNonAllocLinkedListBag<INonAllocSubscription>;

			return BuildConcurrentNonAllocPinger(
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

		public ConcurrentNonAllocPinger BuildConcurrentNonAllocPinger(
			IBag<INonAllocSubscription> subscriptionBag,
			IPool<IBag<INonAllocSubscription>> contextPool)
		{
			ILogger logger =
				loggerResolver?.GetLogger<ConcurrentNonAllocPinger>();

			return new ConcurrentNonAllocPinger(
				subscriptionBag,
				contextPool,
				new object(),
				logger);
		}

		#endregion
	}
}