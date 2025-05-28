using System;
using System.Threading.Tasks;

using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Async;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Async;
using HereticalSolutions.ObjectPools.Configurable.Async.Factories;

namespace HereticalSolutions.ObjectPools.Configurable.Async
{
	public class AsyncConfigurableQueuePool<T>
		: IAsyncPool<T>,
		  IAsyncResizable,
		  IAsyncAllocationCommandResizable<T>,
		  ICleanuppable,
		  IDisposable
	{
		private readonly Queue<T> pool;

		private readonly IAsyncAllocationCommand<T> allocationCommand;

		private readonly AsyncConfigurableQueuePoolFactory factory;

		private int capacity;

		public AsyncConfigurableQueuePool(
			Queue<T> pool,
			IAsyncAllocationCommand<T> allocationCommand,
			AsyncConfigurableQueuePoolFactory factory)
		{
			this.pool = pool;

			this.allocationCommand = allocationCommand;

			this.factory = factory;

			capacity = this.pool.Count;
		}

		#region IPool

		public async Task<T> Pop(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			T result = default(T);

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

		public async Task<T> Pop(
			IPoolPopArgument[] args,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			return await Pop(asyncContext);
		}

		public async Task Push(
			T instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			pool.Enqueue(instance);
		}

		#endregion

		#region IAsyncAllocationResizable

		public async Task Resize(

			//Async tail
			AsyncExecutionContext asyncContext
		)
		{
			capacity = await factory.ResizeAsyncConfigurableQueuePool(
				pool,
				capacity,
				allocationCommand,
				asyncContext);
		}

		#endregion

		#region IAsyncAllocationCommandResizable

		public async Task Resize(
			IAsyncAllocationCommand<T> allocationCommand,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			capacity = await factory.ResizeAsyncConfigurableQueuePool(
				pool,
				capacity,
				allocationCommand,
				asyncContext);
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			foreach (var item in pool)
				if (item is ICleanuppable)
					(item as ICleanuppable).Cleanup();

			pool.Clear();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			foreach (var item in pool)
				if (item is IDisposable)
					(item as IDisposable).Dispose();

			pool.Clear();
		}

		#endregion
	}
}