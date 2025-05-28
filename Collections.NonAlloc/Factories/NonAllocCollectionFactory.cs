using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Collections.Factories;

using HereticalSolutions.ObjectPools;
using HereticalSolutions.ObjectPools.Configurable.Factories;

namespace HereticalSolutions.Collections.NonAlloc.Factories
{
	public class NonAllocCollectionFactory
	{
		private readonly CollectionFactory collectionFactory;

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

		public NonAllocCollectionFactory(
			CollectionFactory collectionFactory,
			ConfigurableStackPoolFactory configurableStackPoolFactory)
		{
			this.collectionFactory = collectionFactory;

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

		public NonAllocCollectionFactory(
			CollectionFactory collectionFactory,
			ConfigurableStackPoolFactory configurableStackPoolFactory,

			AllocationCommandDescriptor initialNodePoolAllocationDescriptor,
			AllocationCommandDescriptor additionalNodePoolAllocationDescriptor)
		{
			this.collectionFactory = collectionFactory;

			this.configurableStackPoolFactory = configurableStackPoolFactory;

			defaultNodePoolInitialAllocationDescriptor =
				initialNodePoolAllocationDescriptor;

			defaultNodePoolAdditionalAllocationDescriptor =
				additionalNodePoolAllocationDescriptor;
		}

		public NonAllocCollectionFactory(
			CollectionFactory collectionFactory,
			ConfigurableStackPoolFactory configurableStackPoolFactory,

			int defaultInitialAllocationAmount,
			int defaultAdditionalAllocationAmount)
		{
			this.collectionFactory = collectionFactory;

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

		#region B+ trees

		public IPool<NonAllocBPlusTreeNode<T>>
			BuildNonAllocBPlusTreeNodePool<T>()
		{
			return BuildNonAllocBPlusTreeNodePool<T>(
				defaultNodePoolInitialAllocationDescriptor,
				defaultNodePoolAdditionalAllocationDescriptor);
		}

		public IPool<NonAllocBPlusTreeNode<T>>
			BuildNonAllocBPlusTreeNodePool<T>(
				AllocationCommandDescriptor initialAllocationDescriptor,
				AllocationCommandDescriptor additionalAllocationDescriptor)
		{
			Func<NonAllocBPlusTreeNode<T>> allocationDelegate = AllocationFactory.
				ActivatorAllocationDelegate<NonAllocBPlusTreeNode<T>>;

			return configurableStackPoolFactory.
				BuildConfigurableStackPool<NonAllocBPlusTreeNode<T>>(
					new AllocationCommand<NonAllocBPlusTreeNode<T>>(
						initialAllocationDescriptor,
						allocationDelegate,
						null),
					new AllocationCommand<NonAllocBPlusTreeNode<T>>(
						additionalAllocationDescriptor,
						allocationDelegate,
						null));
		}

		public NonAllocBPlusTree<T> BuildNonAllocBPlusTree<T>()
		{
			var nodePool = BuildNonAllocBPlusTreeNodePool<T>();

			return new NonAllocBPlusTree<T>(
				nodePool,
				collectionFactory.BPlusTreeDegree);
		}

		public NonAllocBPlusTree<T> BuildNonAllocBPlusTree<T>(
			IPool<NonAllocBPlusTreeNode<T>> nodePool,
			int degree)
		{
			return new NonAllocBPlusTree<T>(
				nodePool,
				degree);
		}

		public IPool<NonAllocBPlusTreeMapNode<TKey, TValue>>
			BuildNonAllocBPlusTreeMapNodePool<TKey, TValue>()
		{
			return BuildNonAllocBPlusTreeMapNodePool<TKey, TValue>(
				defaultNodePoolInitialAllocationDescriptor,
				defaultNodePoolAdditionalAllocationDescriptor);
		}

		public IPool<NonAllocBPlusTreeMapNode<TKey, TValue>>
			BuildNonAllocBPlusTreeMapNodePool<TKey, TValue>(
				AllocationCommandDescriptor initialAllocationDescriptor,
				AllocationCommandDescriptor additionalAllocationDescriptor)
		{
			Func<NonAllocBPlusTreeMapNode<TKey, TValue>> allocationDelegate =
				AllocationFactory.
					ActivatorAllocationDelegate<NonAllocBPlusTreeMapNode<TKey, TValue>>;

			return configurableStackPoolFactory.
				BuildConfigurableStackPool<NonAllocBPlusTreeMapNode<TKey, TValue>>(
					new AllocationCommand<NonAllocBPlusTreeMapNode<TKey, TValue>>(
						initialAllocationDescriptor,
						allocationDelegate,
						null),
					new AllocationCommand<NonAllocBPlusTreeMapNode<TKey, TValue>>(
						additionalAllocationDescriptor,
						allocationDelegate,
						null ));
		}

		public NonAllocBPlusTreeMap<TKey, TValue>
			BuildNonAllocBPlusTreeMap<TKey, TValue>()
		{
			var nodePool = BuildNonAllocBPlusTreeMapNodePool<TKey, TValue>();

			return new NonAllocBPlusTreeMap<TKey, TValue>(
				nodePool,
				collectionFactory.BPlusTreeDegree);
		}

		public NonAllocBPlusTreeMap<TKey, TValue>
			BuildNonAllocBPlusTreeMap<TKey, TValue>(
				IPool<NonAllocBPlusTreeMapNode<TKey, TValue>> nodePool,
				int degree)
		{
			return new NonAllocBPlusTreeMap<TKey, TValue>(
				nodePool,
				degree);
		}

		#endregion
	}
}