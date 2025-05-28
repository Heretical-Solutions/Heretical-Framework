using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.ObjectPools;
using HereticalSolutions.ObjectPools.Configurable.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.SpacePartitioning.Factories
{
	public class NodePoolFactory
	{
		private const int INITIAL_NODE_POOL_SIZE = 128;

		private readonly ConfigurableStackPoolFactory configurableStackPoolFactory;

		private readonly ILoggerResolver loggerResolver;

		public NodePoolFactory(
			ConfigurableStackPoolFactory configurableStackPoolFactory,
			ILoggerResolver loggerResolver)
		{
			this.configurableStackPoolFactory = configurableStackPoolFactory;
			
			this.loggerResolver = loggerResolver;
		}

		public IPool<Node<TValue>> BuildNodePool<TValue>()
		{
			ILogger logger =
				loggerResolver?.GetLogger<Node<TValue>>();

			Func<Node<TValue>> allocationDelegate = () => new Node<TValue>(
				new Node<TValue>[4],
				new List<ValueSpaceData<TValue>>(),
				-1,
				logger);

			return configurableStackPoolFactory.BuildConfigurableStackPool<Node<TValue>>(
				new AllocationCommand<Node<TValue>>(
					new AllocationCommandDescriptor
					{
						Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

						Amount = 5
					},
					allocationDelegate,
					null),
				new AllocationCommand<Node<TValue>>(
					new AllocationCommandDescriptor
					{
						Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

						Amount = INITIAL_NODE_POOL_SIZE
					},
					allocationDelegate,
					null));
		}

		public IPool<ValueSpaceData<TValue>> BuildValueDataPool<TValue>()
		{
			Func<ValueSpaceData<TValue>> allocationDelegate =
				() => new ValueSpaceData<TValue>();

			return configurableStackPoolFactory.
				BuildConfigurableStackPool<ValueSpaceData<TValue>>(
					new AllocationCommand<ValueSpaceData<TValue>>(
						new AllocationCommandDescriptor
						{
							Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
	
							Amount = 5
						},
						allocationDelegate,
						null),
					new AllocationCommand<ValueSpaceData<TValue>>(
						new AllocationCommandDescriptor
						{
							Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
	
							Amount = INITIAL_NODE_POOL_SIZE
						},
						allocationDelegate,
						null));
		}
	}
}