using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.ObjectPools.Configurable.Factories;

namespace HereticalSolutions.Bags.NonAlloc.Factories
{
	public class NonAllocLinkedListBagFactory
	{
		private readonly ConfigurableStackPoolFactory configurableStackPoolFactory;

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

		public NonAllocLinkedListBagFactory(
			ConfigurableStackPoolFactory configurableStackPoolFactory)
		{
			this.configurableStackPoolFactory = configurableStackPoolFactory;

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

		public NonAllocLinkedListBagFactory(
			ConfigurableStackPoolFactory configurableStackPoolFactory,

			AllocationCommandDescriptor initialNodePoolAllocationDescriptor,
			AllocationCommandDescriptor additionalNodePoolAllocationDescriptor)
		{
			this.configurableStackPoolFactory = configurableStackPoolFactory;

			defaultNodePoolInitialAllocationDescriptor = 
				initialNodePoolAllocationDescriptor;
			
			defaultNodePoolAdditionalAllocationDescriptor = 
				additionalNodePoolAllocationDescriptor;
		}

		public NonAllocLinkedListBagFactory(
			ConfigurableStackPoolFactory configurableStackPoolFactory,

			int defaultInitialAllocationAmount,
			int defaultAdditionalAllocationAmount)
		{
			this.configurableStackPoolFactory = configurableStackPoolFactory;

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

		#region Non alloc linked list bag

		public NonAllocLinkedListBag<T> BuildNonAllocLinkedListBag<T>()
		{
			return BuildNonAllocLinkedListBag<T>(
				defaultNodePoolInitialAllocationDescriptor,
				defaultNodePoolAdditionalAllocationDescriptor);
		}

		public NonAllocLinkedListBag<T> BuildNonAllocLinkedListBag<T>(
			AllocationCommandDescriptor initialAllocationDescriptor,
			AllocationCommandDescriptor additionalAllocationDescriptor)
		{
			var linkedList = new LinkedList<T>();

			Func<LinkedListNode<T>> allocationDelegate =
				() => new LinkedListNode<T>(default);

			return new NonAllocLinkedListBag<T>(
				linkedList,
				configurableStackPoolFactory.
					BuildConfigurableStackPool<LinkedListNode<T>>(
						new AllocationCommand<LinkedListNode<T>>(
							initialAllocationDescriptor,
							allocationDelegate,
							null),
						new AllocationCommand<LinkedListNode<T>>(
							additionalAllocationDescriptor,
							allocationDelegate,
							null)));
		}

		#endregion
	}
}