using System;

using System.Collections.Generic;

namespace HereticalSolutions.ObjectPools
{
	public class LinkedListPool<T>
		: IPool<T>,
		  IResizable,
		  INumericalResizable
	{
		private readonly LinkedList<T> pool;

		private readonly Func<T> allocationDelegate;

		public LinkedListPool(
			LinkedList<T> pool,
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
			Resize(1);
		}

		#endregion

		#region INumericalResizable

		public void Resize(
			int additionalAmount)
		{
			for (int i = 0; i < additionalAmount; i++)
			{
				T newElement = allocationDelegate();

				pool.AddFirst(newElement);
			}
		}

		#endregion
	}
}