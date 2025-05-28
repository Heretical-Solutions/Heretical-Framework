using System;

using System.Collections.Generic;

namespace HereticalSolutions.ObjectPools
{
	public class QueuePool<T>
		: IPool<T>,
		  IResizable,
		  INumericalResizable
	{
		private readonly Queue<T> pool;

		private readonly Func<T> allocationDelegate;

		public QueuePool(
			Queue<T> pool,
			Func<T> allocationDelegate)
		{
			this.pool = pool;

			this.allocationDelegate = allocationDelegate;
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

		#region IResizable

		public void Resize()
		{
			Resize(1);
		}

		#endregion

		#region INumericalResizable

		public void Resize(
			int additionalAmount)
		{
			for (int i = 0; i < additionalAmount; i++)
			{
				pool.Enqueue(allocationDelegate());
			}
		}

		#endregion
	}
}