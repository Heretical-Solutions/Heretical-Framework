using System.Threading;

using HereticalSolutions.Allocations;

using HereticalSolutions.Bags.NonAlloc.Factories;

namespace HereticalSolutions.Bags.NonAlloc.Concurrent.Factories
{
	public class ConcurrentNonAllocLinkedListBagFactory
	{
		private readonly NonAllocLinkedListBagFactory nonAllocLinkedListBagFactory;

		#region Node pool

		private const int DEFAULT_NODE_POOL_INITIAL_ALLOCATION_AMOUNT = 8;

		private const int DEFAULT_NODE_POOL_ADDITIONAL_ALLOCATION_AMOUNT = 8;

		protected AllocationCommandDescriptor
			defaultNodePoolInitialAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = DEFAULT_NODE_POOL_INITIAL_ALLOCATION_AMOUNT
				};

		protected AllocationCommandDescriptor
			defaultNodePoolAdditionalAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = DEFAULT_NODE_POOL_ADDITIONAL_ALLOCATION_AMOUNT
				};

		#endregion

		public ConcurrentNonAllocLinkedListBagFactory(
			NonAllocLinkedListBagFactory nonAllocLinkedListBagFactory)
		{
			this.nonAllocLinkedListBagFactory = nonAllocLinkedListBagFactory;

			defaultNodePoolInitialAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = DEFAULT_NODE_POOL_INITIAL_ALLOCATION_AMOUNT
				};

			defaultNodePoolAdditionalAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = DEFAULT_NODE_POOL_ADDITIONAL_ALLOCATION_AMOUNT
				};
		}

		public ConcurrentNonAllocLinkedListBagFactory(
			NonAllocLinkedListBagFactory nonAllocLinkedListBagFactory,

			AllocationCommandDescriptor initialNodePoolAllocationDescriptor,
			AllocationCommandDescriptor additionalNodePoolAllocationDescriptor)
		{
			this.nonAllocLinkedListBagFactory = nonAllocLinkedListBagFactory;

			defaultNodePoolInitialAllocationDescriptor =
				initialNodePoolAllocationDescriptor;

			defaultNodePoolAdditionalAllocationDescriptor =
				additionalNodePoolAllocationDescriptor;
		}

		public ConcurrentNonAllocLinkedListBagFactory(
			NonAllocLinkedListBagFactory nonAllocLinkedListBagFactory,

			int defaultInitialAllocationAmount,
			int defaultAdditionalAllocationAmount)
		{
			this.nonAllocLinkedListBagFactory = nonAllocLinkedListBagFactory;

			defaultNodePoolInitialAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = defaultInitialAllocationAmount
				};

			defaultNodePoolAdditionalAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = defaultAdditionalAllocationAmount
				};
		}

		#region Concurrent non alloc linked list bag

		public ConcurrentNonAllocLinkedListBag<T>
			BuildConcurrentNonAllocLinkedListBag<T>()
		{
			return BuildConcurrentNonAllocLinkedListBag<T>(
				defaultNodePoolInitialAllocationDescriptor,
				defaultNodePoolAdditionalAllocationDescriptor);
		}

		public ConcurrentNonAllocLinkedListBag<T>
			BuildConcurrentNonAllocLinkedListBag<T>(
				AllocationCommandDescriptor initialAllocationDescriptor,
				AllocationCommandDescriptor additionalAllocationDescriptor)
		{
			return new ConcurrentNonAllocLinkedListBag<T>(
				nonAllocLinkedListBagFactory.
					BuildNonAllocLinkedListBag<T>(
						initialAllocationDescriptor,
						additionalAllocationDescriptor),
				new SemaphoreSlim(1, 1));
		}

		#endregion
	}
}