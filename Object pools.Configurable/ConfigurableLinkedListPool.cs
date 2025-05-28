using System;

using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Configurable.Factories;

namespace HereticalSolutions.ObjectPools.Configurable
{
	public class ConfigurableLinkedListPool<T>
		: IPool<T>,
		  IResizable,
		  IAllocationCommandResizable<T>,
		  ICleanuppable,
		  IDisposable
	{
		private readonly LinkedList<T> pool;

		private readonly IAllocationCommand<T> allocationCommand;

		private readonly ConfigurableLinkedListPoolFactory factory;

		private int capacity;

		public ConfigurableLinkedListPool(
			LinkedList<T> pool,
			IAllocationCommand<T> allocationCommand,
			ConfigurableLinkedListPoolFactory factory)
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
				result = pool.First.Value;

				pool.RemoveFirst();
			}
			else
			{
				Resize();

				result = pool.First.Value;

				pool.RemoveFirst();
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
			pool.AddFirst(instance);
		}

		#endregion

		#region IResizable

		public void Resize()
		{
			capacity = factory.ResizeConfigurableLinkedListPool(
				pool,
				capacity,
				allocationCommand);
		}

		#endregion

		#region IAllocationCommandResizable

		public void Resize(
			IAllocationCommand<T> allocationCommand)
		{
			capacity = factory.ResizeConfigurableLinkedListPool(
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