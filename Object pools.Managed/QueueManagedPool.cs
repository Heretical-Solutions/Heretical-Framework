using System;

using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Managed.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Managed
{
	public class QueueManagedPool<T>
		: AManagedPool<T>
	{
		protected readonly QueueManagedPoolFactory queueManagedPoolFactory;

		protected readonly Queue<IPoolElementFacade<T>> pool;

		protected readonly ILogger logger;

		public QueueManagedPool(
			QueueManagedPoolFactory queueManagedPoolFactory,

			IAllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
			IAllocationCommand<T> valueAllocationCommand,
			
			Queue<IPoolElementFacade<T>> pool,

			ILogger logger)
			: base(
				facadeAllocationCommand,
				valueAllocationCommand)
		{
			this.queueManagedPoolFactory = queueManagedPoolFactory;

			this.pool = pool;

			this.logger = logger;

			capacity = this.pool.Count;
		}

		public override IPoolElementFacade<T> PopFacade()
		{
			IPoolElementFacade<T> result = null;

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

		public override void PushFacade(
			IPoolElementFacade<T> instance)
		{
			pool.Enqueue(instance);
		}

		public override void Resize()
		{
			capacity = queueManagedPoolFactory.ResizeQueueManagedPool(
				pool,
				capacity,

				facadeAllocationCommand,
				valueAllocationCommand,

				true);
		}

		public override void Resize(
			IAllocationCommand<T> allocationCommand,
			bool newValuesAreInitialized)
		{
			capacity = queueManagedPoolFactory.ResizeQueueManagedPool(
				pool,
				capacity,

				facadeAllocationCommand,
				allocationCommand,

				newValuesAreInitialized);
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