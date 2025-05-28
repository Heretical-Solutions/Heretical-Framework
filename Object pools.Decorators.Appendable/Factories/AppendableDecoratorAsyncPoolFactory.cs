using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Async;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.ObjectPools.Managed.Async;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Decorators.Appendable.Factories
{
	public class AppendableDecoratorAsyncPoolFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public AppendableDecoratorAsyncPoolFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		public AppendableAsyncDecoratorManagedPool<T>
			BuildAppendableAsyncDecoratorManagedPool<T>(
				IAsyncManagedPool<T> innerPool)
		{
			var logger = loggerResolver?
				.GetLogger<AppendableAsyncDecoratorManagedPool<T>>();

			IAsyncManagedPoolInternal<T> innerPoolInternal =
				innerPool as IAsyncManagedPoolInternal<T>;

			Func<Task<T>> allocationDelegate =
				async () =>
				{
					var result = AllocationFactory.NullAllocationDelegate<T>();

					return result;
				};

			return new AppendableAsyncDecoratorManagedPool<T>(
				innerPool,
				new List<IAsyncPoolElementFacade<T>>(),
				new AsyncAllocationCommand<T>(
					new AllocationCommandDescriptor
					{
						Rule = EAllocationAmountRule.ADD_ONE,

						Amount = 1
					},
					allocationDelegate,
					innerPoolInternal.ValueAllocationCommand.AllocationCallback),
				logger);
		}
	}
}