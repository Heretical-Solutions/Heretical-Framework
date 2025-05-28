using System;

using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Configurable.Factories;

namespace HereticalSolutions.ObjectPools.Configurable
{
	public class ConfigurableQueuePool<T>
		: IPool<T>,
		  IResizable,
		  IAllocationCommandResizable<T>,
		  ICleanuppable,
		  IDisposable
	{
		private readonly Queue<T> pool;

		private readonly IAllocationCommand<T> allocationCommand;

		private readonly ConfigurableQueuePoolFactory factory;

		private int capacity;

		public ConfigurableQueuePool(
			Queue<T> pool,
			IAllocationCommand<T> allocationCommand,
			ConfigurableQueuePoolFactory factory)
		{
			this.pool = pool;

			this.allocationCommand = allocationCommand;

			this.factory = factory;

			capacity = this.pool.Count;
		}

		#region IPool

		public T Pop()
		{
			T result = default(T);

			if (pool.Count != 0)
			{
				result = pool.Dequeue();
			}
			else
			{
				Resize();

				result = pool.Dequeue();
			}

			return result;
		}

		public T Pop(
			IPoolPopArgument[] args)
		{
			return Pop();
		}

		public void Push(
			T instance)
		{
			pool.Enqueue(instance);
		}

		#endregion

		#region IAllocationResizable

		public void Resize()
		{
			capacity = factory.ResizeConfigurableQueuePool(
				pool,
				capacity,
				allocationCommand);
		}

		#endregion

		#region IAllocationCommandResizable

		public void Resize(
			IAllocationCommand<T> allocationCommand)
		{
			capacity = factory.ResizeConfigurableQueuePool(
				pool,
				capacity,
				allocationCommand);
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