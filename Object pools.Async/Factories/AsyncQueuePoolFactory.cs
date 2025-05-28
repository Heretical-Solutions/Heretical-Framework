using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.ObjectPools.Async.Factories
{
	public class AsyncQueuePoolFactory
	{
		#region Build

		public async Task<AsyncQueuePool<T>> BuildAsyncQueuePool<T>(
			Func<Task<T>> allocationDelegate,
			int initialAmount,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var queue = new Queue<T>();

			await PerformInitialAllocation<T>(
				queue,
				allocationDelegate,
				initialAmount,
				asyncContext);

			return new AsyncQueuePool<T>(
				queue,
				allocationDelegate);
		}

		private async Task PerformInitialAllocation<T>(
			Queue<T> queue,
			Func<Task<T>> allocationDelegate,
			int initialAmount,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = await allocationDelegate();

				queue.Enqueue(
					newElement);
			}
		}

		#endregion
	}
}