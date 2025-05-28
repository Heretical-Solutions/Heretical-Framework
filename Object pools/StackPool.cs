using System;

using System.Collections.Generic;

namespace HereticalSolutions.ObjectPools
{
	public class StackPool<T>
		: IPool<T>,
		  IResizable,
		  INumericalResizable
	{
		private readonly Stack<T> pool;

		private readonly Func<T> allocationDelegate;

		public StackPool(
			Stack<T> pool,
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
				result = pool.Pop();
			}
			else
			{
				Resize();

				result = pool.Pop();
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
			pool.Push(instance);
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

				pool.Push(newElement);
			}
		}

		#endregion
	}
}