using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations.Async;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Managed.Async.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Managed.Async
{
	public class AsyncStackManagedPool<T>
		: AAsyncManagedPool<T>
	{
		protected readonly AsyncStackManagedPoolFactory asyncStackManagedPoolFactory;

		protected readonly Stack<IAsyncPoolElementFacade<T>> pool;

		protected readonly ILogger logger;

		public AsyncStackManagedPool(
			AsyncStackManagedPoolFactory asyncStackManagedPoolFactory,

			Stack<IAsyncPoolElementFacade<T>> pool,

			IAsyncAllocationCommand<IAsyncPoolElementFacade<T>> facadeAllocationCommand,
			IAsyncAllocationCommand<T> valueAllocationCommand,

			ILogger logger)
			: base(
				facadeAllocationCommand,
				valueAllocationCommand)
		{
			this.asyncStackManagedPoolFactory = asyncStackManagedPoolFactory;

			this.pool = pool;

			this.logger = logger;

			capacity = this.pool.Count;
		}

		public override async Task<IAsyncPoolElementFacade<T>> PopFacade(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncPoolElementFacade<T> result = null;

			if (pool.Count != 0)
			{
				result = pool.Pop();
			}
			else
			{
				await Resize(asyncContext);

				result = pool.Pop();
			}

			return result;
		}

		public override async Task PushFacade(
			IAsyncPoolElementFacade<T> instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			pool.Push(instance);
		}

		public override async Task Resize(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			capacity = await asyncStackManagedPoolFactory.ResizeAsyncStackManagedPool(
				pool,
				capacity,

				facadeAllocationCommand,
				valueAllocationCommand,

				true,
				
				asyncContext);
		}

		public override async Task Resize(
			IAsyncAllocationCommand<T> allocationCommand,
			bool newValuesAreInitialized,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			capacity = await asyncStackManagedPoolFactory.ResizeAsyncStackManagedPool(
				pool,
				capacity,

				facadeAllocationCommand,
				allocationCommand,

				newValuesAreInitialized,
				
				asyncContext);
		}

		public override void Cleanup()
		{
			foreach (var item in pool)
				if (item is ICleanuppable)
					(item as ICleanuppable).Cleanup();

			pool.Clear();
		}
		public override void Dispose()
		{
			foreach (var item in pool)
				if (item is IDisposable)
					(item as IDisposable).Dispose();

			pool.Clear();
		}
	}
}