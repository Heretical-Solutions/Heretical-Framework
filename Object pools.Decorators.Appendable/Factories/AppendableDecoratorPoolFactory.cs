using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.ObjectPools.Managed;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Decorators.Appendable.Factories
{
	public class AppendableDecoratorPoolFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public AppendableDecoratorPoolFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		public AppendableDecoratorManagedPool<T>
			BuildAppendableDecoratorManagedPool<T>(
				IManagedPool<T> innerPool)
		{
			var logger = loggerResolver?
				.GetLogger<AppendableDecoratorManagedPool<T>>();

			IManagedPoolInternal<T> innerPoolInternal =
				innerPool as IManagedPoolInternal<T>;

			return new AppendableDecoratorManagedPool<T>(
				innerPool,
				new List<IPoolElementFacade<T>>(),
				new AllocationCommand<T>(
					new AllocationCommandDescriptor
					{
						Rule = EAllocationAmountRule.ADD_ONE,

						Amount = 1
					},
					AllocationFactory.NullAllocationDelegate<T>,
					innerPoolInternal.ValueAllocationCommand.AllocationCallback),
				logger);
		}
	}
}