using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.ObjectPools;
using HereticalSolutions.ObjectPools.Builders;

using HereticalSolutions.ObjectPools.Configurable.Factories;
using HereticalSolutions.ObjectPools.Configurable.Builders;

using HereticalSolutions.ObjectPools.Decorators.Cleanup.Factories;
using HereticalSolutions.ObjectPools.Decorators.Cleanup.Builders;

namespace HereticalSolutions.Hierarchy.Factories
{
	public class HierarchyFactory
	{
		private readonly ConfigurableStackPoolFactory configurableStackPoolFactory;

		private readonly CleanupDecoratorPoolFactory cleanupDecoratorPoolFactory;

		#region Node pool

		private const int
			DEFAULT_NODE_POOL_INITIAL_ALLOCATION_AMOUNT = 8;

		private const int
			DEFAULT_NODE_POOL_ADDITIONAL_ALLOCATION_AMOUNT = 8;

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

		public HierarchyFactory(
			ConfigurableStackPoolFactory configurableStackPoolFactory,
			CleanupDecoratorPoolFactory cleanupDecoratorPoolFactory)
		{
			this.configurableStackPoolFactory = configurableStackPoolFactory;
			this.cleanupDecoratorPoolFactory = cleanupDecoratorPoolFactory;

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

		public HierarchyFactory(
			ConfigurableStackPoolFactory configurableStackPoolFactory,
			CleanupDecoratorPoolFactory cleanupDecoratorPoolFactory,
			
			AllocationCommandDescriptor defaultNodePoolInitialAllocationDescriptor,
			AllocationCommandDescriptor defaultNodePoolAdditionalAllocationDescriptor)
		{
			this.configurableStackPoolFactory = configurableStackPoolFactory;
			this.cleanupDecoratorPoolFactory = cleanupDecoratorPoolFactory;

			this.defaultNodePoolInitialAllocationDescriptor = 
				defaultNodePoolInitialAllocationDescriptor;

			this.defaultNodePoolAdditionalAllocationDescriptor = 
				defaultNodePoolAdditionalAllocationDescriptor;
		}

		public HierarchyNode<TContents> BuildHierarchyNode<TContents>()
		{
			return new HierarchyNode<TContents>(
				new List<IReadOnlyHierarchyNode<TContents>>());
		}

		public IPool<List<IReadOnlyHierarchyNode<T>>> BuildHierarchyNodeListPool<T>(
			PoolBuilder<List<IReadOnlyHierarchyNode<T>>> poolBuilder)
		{
			return BuildHierarchyNodeListPool<T>(
				poolBuilder,

				defaultNodePoolInitialAllocationDescriptor,
				defaultNodePoolAdditionalAllocationDescriptor);
		}

		public IPool<List<IReadOnlyHierarchyNode<T>>> BuildHierarchyNodeListPool<T>(
			PoolBuilder<List<IReadOnlyHierarchyNode<T>>> poolBuilder,


			AllocationCommandDescriptor initialAllocationDescriptor,
			AllocationCommandDescriptor additionalAllocationDescriptor)
		{
			//Func<List<IReadOnlyHierarchyNode<T>>> allocationDelegate = AllocationFactory
			//	.ActivatorAllocationDelegate<List<IReadOnlyHierarchyNode<T>>>;

			return poolBuilder
				.New()
				.ConfigurableStackPool(
					configurableStackPoolFactory)
				.WithInitial(
					initialAllocationDescriptor)
				.WithAdditional(
					additionalAllocationDescriptor)
				.WithActivatorAllocation()
				.DecoratedWithListCleanup(
					cleanupDecoratorPoolFactory)
				.Build();
		}
	}
}