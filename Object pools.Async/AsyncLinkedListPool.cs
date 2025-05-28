using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.ObjectPools.Async
{
	public class AsyncLinkedListPool<T>
		: IAsyncPool<T>,
		  IAsyncResizable,
		  IAsyncNumericalResizable
	{
		private readonly LinkedList<T> pool;

		private readonly Func<Task<T>> allocationDelegate;

		public AsyncLinkedListPool(
			LinkedList<T> pool,
			Func<Task<T>> allocationDelegate)
		{
			this.pool = pool;

			this.allocationDelegate = allocationDelegate;
		}

		#region IPool

		public async Task<T> Pop(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			T result = default(T);

			if (pool.Count != 0)
			{
				result = pool.First.Value;

				pool.RemoveFirst();
			}
			else
			{
				await Resize(asyncContext);

				result = pool.First.Value;

				pool.RemoveFirst();
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
			pool.AddFirst(instance);
		}

		#endregion

		#region IAsyncResizable

		public async Task Resize(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await Resize(
				1,
				asyncContext);
		}

		#endregion

		#region IAsyncNumericalResizable

		public async Task Resize(
			int additionalAmount,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			for (int i = 0; i < additionalAmount; i++)
			{
				T newElement = await allocationDelegate();

				pool.AddFirst(newElement);
			}
		}

		#endregion
	}
}