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
	public class AsyncQueueManagedPool<T>
		: AAsyncManagedPool<T>
	{
		protected readonly AsyncQueueManagedPoolFactory asyncQueueManagedPoolFactory;

		protected readonly Queue<IAsyncPoolElementFacade<T>> pool;

		protected readonly ILogger logger;

		public AsyncQueueManagedPool(
			AsyncQueueManagedPoolFactory asyncQueueManagedPoolFactory,

			IAsyncAllocationCommand<IAsyncPoolElementFacade<T>> facadeAllocationCommand,
			IAsyncAllocationCommand<T> valueAllocationCommand,

			Queue<IAsyncPoolElementFacade<T>> pool,

			ILogger logger)
			: base(
				facadeAllocationCommand,
				valueAllocationCommand)
		{
			this.asyncQueueManagedPoolFactory = asyncQueueManagedPoolFactory;

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
				result = pool.Dequeue();
			}
			else
			{
				await Resize(asyncContext);

				result = pool.Dequeue();
			}

			return result;
		}

		public override async Task PushFacade(
			IAsyncPoolElementFacade<T> instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			pool.Enqueue(instance);
		}

		public override async Task Resize(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			capacity = await asyncQueueManagedPoolFactory.ResizeAsyncQueueManagedPool(
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
			capacity = await asyncQueueManagedPoolFactory.ResizeAsyncQueueManagedPool(
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